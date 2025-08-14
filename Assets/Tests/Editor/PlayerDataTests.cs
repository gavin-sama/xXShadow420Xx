using NUnit.Framework; // Unity uses NUnit for testing
using UnityEngine;

public class PlayerDataHealthTests
{
    [Test]
    public void Player_Health_Is_100_By_Default()
    {
        // Arrange
        var playerData = new PlayerData();

        // Act
        float health = playerData.healthPoints;

        // Assert
        Assert.AreEqual(100f, health, "Default health should be 100.");
    }

}

