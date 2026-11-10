
namespace RambutanSmartGarden.Api.Common.Results;

public sealed record ApiResponse<T>(
    T Data,
    string? Message = null
);
