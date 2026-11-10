
namespace RambutanSmartGarden.Api.Features.Trees.DTOs;

public sealed record TreeReplacementResponse(
    Guid Id,
    Guid GardenId,
    Guid OldTreeId,
    Guid NewTreeId,
    Guid PlantingSpotId,
    string OldTreePreviousStatus,
    string Reason,
    DateTime ReplacedAt,
    Guid PerformedByUserId,
    string RowVersion,
    TreeResponse NewTree
);
