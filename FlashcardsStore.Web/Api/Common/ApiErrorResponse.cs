namespace FlashcardsStore.Web.Api.Common;

/// <summary>Standard error payload returned by all mobile API endpoints.</summary>
public sealed record ApiErrorResponse(string Error);
