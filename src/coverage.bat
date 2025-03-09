
del /Q /S Tests\TestResults\*.*

dotnet test --collect:"XPlat Code Coverage;Exclude=[Mapping.*]" --verbosity quiet

reportgenerator -reports:"Tests\TestResults\**\coverage.cobertura.xml" -targetdir:"Tests\TestResults" -reporttypes:Html

Tests\TestResults\index.html
