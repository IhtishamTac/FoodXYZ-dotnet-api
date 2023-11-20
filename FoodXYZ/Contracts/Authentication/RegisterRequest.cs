namespace FoodXYZ.Contracts.Authentication;

public record RegisterRequest(
    string Name,
    string Username,
    string Address,
    string Password,
    string Password_confirmation
);