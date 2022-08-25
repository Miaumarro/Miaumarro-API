using MiauDatabase.Enums;

namespace MiauAPI.Models.Requests;

public sealed record UpdatePurchaseRequest(int Id, int UserId, PurchaseStatus Status);