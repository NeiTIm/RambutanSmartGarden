
namespace RambutanSmartGarden.Api.Features.Gardens.Models;

public enum GardenMemberMutationStatus
{
    Succeeded,
    Forbidden,
    NotFound,
    InvalidRole,
    SelfModificationNotAllowed,
    LastAdminProtected,
    NoChange,
    ConcurrencyConflict
}
