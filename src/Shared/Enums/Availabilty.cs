namespace Drogecode.Knrm.Oefenrooster.Shared.Enums;

// Typo, but used in both server and client and can not be changed without a breaking update
// This is the wrong version but keeping it until all clients are v0.3.50 or above
// Availability
public enum Availabilty
{
    None = 0,
    Available = 1,
    NotAvailable = 2,
    Maybe = 3,
}
