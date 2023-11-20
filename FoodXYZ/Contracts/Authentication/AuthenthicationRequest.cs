namespace FoodXYZ.Contracts.Authentication;

public record AuthenticationRequest(
    string Name,
    string Username,
    string Address,
    string Image,
    string Token
);