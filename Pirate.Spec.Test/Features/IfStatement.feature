Feature: IfStatement

    
Scenario: If statement with a single condition
    Given the following pirate code:
        """
        if 3 == 3 {
            IO.print("Ahoy!");
        }
        """
    
    When the code is executed
    Then the result should be "Ahoy!"