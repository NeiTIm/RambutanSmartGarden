
using System.Security.Claims;
using System.Text.RegularExpressions;
using RambutanSmartGarden.Api.Features.Trees.DTOs;
using RambutanSmartGarden.Api.Features.Trees.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Trees.Services.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Authorization;

namespace RambutanSmartGarden.Api.Features.Trees.Services;

public sealed class TreeReplacementService
    : ITreeReplacementService
{
    private readonly ITreeReplacementRepository _repository;
    private readonly IGardenAccessService _gardenAccess;

    public TreeReplacementService(
        ITreeReplacementRepository repository,
        IGardenAccessService gardenAccess)
    {
        _repository = repository;
        _gardenAccess = gardenAccess;
    }

    public async Task<TreeReplacementListResult> GetAllAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CancellationToken cancellationToken)
    {
        if (!await CanAccessAsync(
                principal,
                gardenId,
                GardenPermission.View,
                cancellationToken))
        {
            return new(ReplaceTreeStatus.NotFound);
        }

        var replacements = await _repository.GetAllAsync(
            gardenId,
            cancellationToken);

        return new(
            ReplaceTreeStatus.Succeeded,
            replacements);
    }

    public async Task<ReplaceTreeResult> GetByIdAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid replacementId,
        CancellationToken cancellationToken)
    {
        if (replacementId == Guid.Empty ||
            !await CanAccessAsync(
                principal,
                gardenId,
                GardenPermission.View,
                cancellationToken))
        {
            return new(ReplaceTreeStatus.NotFound);
        }

        var replacement = await _repository.FindAsync(
            gardenId,
            replacementId,
            cancellationToken);

        return replacement is null
            ? new(ReplaceTreeStatus.NotFound)
            : new(ReplaceTreeStatus.Succeeded, replacement);
    }

    public async Task<ReplaceTreeResult> ReplaceAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid oldTreeId,
        ReplaceTreeRequest request,
        CancellationToken cancellationToken)
    {
        if (oldTreeId == Guid.Empty ||
            !await CanAccessAsync(
                principal,
                gardenId,
                GardenPermission.ManageGarden,
                cancellationToken))
        {
            return new(ReplaceTreeStatus.NotFound);
        }

        if (!IsValidRequest(request) ||
            !TryDecodeRowVersion(
                request.OldTreeRowVersion,
                out var originalRowVersion))
        {
            return new(ReplaceTreeStatus.InvalidRequest);
        }

        if (!Guid.TryParse(
                principal.FindFirst("sub")?.Value,
                out var actorUserId))
        {
            return new(ReplaceTreeStatus.NotFound);
        }

        return await _repository.ReplaceAsync(
            gardenId,
            oldTreeId,
            actorUserId,
            request,
            originalRowVersion,
            cancellationToken);
    }

    private Task<bool> CanAccessAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        GardenPermission permission,
        CancellationToken cancellationToken)
    {
        if (gardenId == Guid.Empty)
        {
            return Task.FromResult(false);
        }

        return _gardenAccess.HasPermissionAsync(
            principal,
            gardenId,
            permission,
            cancellationToken);
    }

    private static bool IsValidRequest(
        ReplaceTreeRequest request)
    {
        var code = request.NewTreeCode?.Trim();
        var reason = request.Reason?.Trim();
        var name = request.NewTreeName?.Trim();
        var notes = request.NewTreeNotes?.Trim();

        if (string.IsNullOrWhiteSpace(code) ||
            code.Length > 30 ||
            !Regex.IsMatch(code, @"^[a-zA-Z0-9_-]+$") ||
            request.NewPlantSpeciesId == Guid.Empty ||
            request.NewTreeVarietyId == Guid.Empty ||
            string.IsNullOrWhiteSpace(reason) ||
            reason.Length is < 3 or > 500 ||
            name?.Length > 150 ||
            notes?.Length > 2000)
        {
            return false;
        }

        if (request.NewTreePlantedAt is DateOnly plantedAt &&
            plantedAt > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return false;
        }

        return true;
    }

    private static bool TryDecodeRowVersion(
        string? encoded,
        out byte[] rowVersion)
    {
        rowVersion = [];

        if (string.IsNullOrWhiteSpace(encoded))
        {
            return false;
        }

        try
        {
            rowVersion = Convert.FromBase64String(encoded);
            return rowVersion.Length == 8;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
