# Cleaning Robot

## Description

MyQ has decided to launch a new automated cleaning robot to the market. The robot is designed to clean all surfaces in a room automatically, without manual intervention. The robot receives information about the room as a set of cells, where each cell represents:
- A cleanable space of 1 by 1 that can be occupied and cleaned (S).
- A column of 1 by 1 which can’t be occupied or cleaned (C).
- A wall represented by an empty cell (null) or by being outside the matrix.

The map is provided as a matrix of m by n, where each element of the matrix is one of these items or empty (null).

## Example Map


```
[
  ["S", "S", "S", "S"],
  ["S", "S", "C", "S"],
  ["S", "S", "S", "S"],
  ["S", null, "S", "S"]
]

```

## Commands

The robot recognizes a set of basic commands, each consuming a certain amount of battery:
- **Turn Left (TL)**: Turns the robot 90 degrees to the left. Consumes 1 unit of battery.
- **Turn Right (TR)**: Turns the robot 90 degrees to the right. Consumes 1 unit of battery.
- **Advance (A)**: Moves the robot one cell forward. Consumes 2 units of battery.
- **Back (B)**: Moves the robot one cell backward without changing direction. Consumes 3 units of battery.
- **Clean (C)**: Cleans the current cell. Consumes 5 units of battery.

## Back Off Strategy

If the robot hits an obstacle (a column or a wall) or runs out of battery, it initiates a back off strategy:
1. Perform [TR, A, TL]. If an obstacle is hit, drop the rest of the sequence and
2. Perform [TR, A, TR]. If an obstacle is hit, drop the rest of the sequence and
3. Perform [TR, A, TR]. If an obstacle is hit, drop the rest of the sequence and
4. Perform [TR, B, TR, A]. If an obstacle is hit, drop the rest of the sequence and
5. Perform [TL, TL, A]. If an obstacle is hit, the robot is considered stuck. Skip all remaining commands and finish the program.

## Input JSON Format

The robot receives a JSON file with the following format:


```
{
  "map": [
    ["S", "S", "S", "S"],
    ["S", "S", "C", "S"],
    ["S", "S", "S", "S"],
    ["S", null, "S", "S"]
  ],
  "start": {"X": 3, "Y": 0, "facing": "N"},
  "commands": ["TL", "A", "C", "A", "C", "TR", "A", "C"],
  "battery": 80
}

```

## Output JSON Format

Upon execution, the robot produces a result JSON describing the cleaning results:


```
{
  "visited": [{"X": 1, "Y": 0}, {"X": 2, "Y": 0}, {"X": 3, "Y": 0}],
  "cleaned": [{"X": 1, "Y": 0}, {"X": 2, "Y": 1}],
  "final": {"X": 2, "Y": 0, "facing": "E"},
  "battery": 54
}

```

## How to Build and Run

### Prerequisites

- .NET 9 SDK
- Visual Studio 2022

### Build

1. Open the solution in Visual Studio 2022.
2. Build the solution using __Build > Build Solution__.

### Run

To run the program, use the following command:


```
cleaning_robot <source.json> <result.json>

```

- `<source.json>`: Path to the input JSON file.
- `<result.json>`: Path to the output JSON file where results will be written.

### Example


```
cleaning_robot input.json output.json

```

## Unit Tests

Unit tests are provided to assert the robot behaves correctly under various conditions. To run the tests:

1. Open the solution in Visual Studio 2022.
2. Run the tests using __Test > Run All Tests__.


## Project Structure

- `CleaningRobot/Program.cs`: Entry point of the application.
- `CleaningRobot/CleaningRobot.csproj`: Main project file for the application, includes configuration and dependency references.
- `CleaningRobot.Entities/CleaningRobot.Entities.csproj`: Contains the entity definitions used across the application.
- `CleaningRobot.InterfaceAdapters/CleaningRobot.InterfaceAdapters.csproj`: Contains the logic for orchestrating the robot's actions and interfacing with other components.
- `CleaningRobot.UseCases/CleaningRobot.UseCases.csproj`: Handles the execution of robot commands and use case logic.
- `CleaningRobot.Tests/CleaningRobot.Tests.csproj`: Contains unit tests to ensure the robot behaves correctly under various conditions.

## Conclusion

This project demonstrates a command line program that simulates an autonomous cleaning robot. The robot processes commands, handles obstacles with a back off strategy, and outputs the results in a specified format. The solution includes unit tests to ensure the robot behaves correctly under various conditions.
