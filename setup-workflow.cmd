@echo off
setlocal

if "%1"=="" (
    echo Usage: %~nx0 ^<destination^>
    exit /b 1
)

set "dest=%~1"

echo Creating workflow structure in "%dest%"...

mkdir "%dest%\.opencode\agents" 2>nul
mkdir "%dest%\.opencode\command" 2>nul
mkdir "%dest%\.opencode\skills\build-project\scripts" 2>nul
mkdir "%dest%\.opencode\skills\second-opinion" 2>nul

> "%dest%\AGENTS.md" (
    echo An ASP.NET Core project with Minimal APIs and Blazor
    echo.
    echo ### Conventions:
    echo - Use `dotnet` CLI for all commands
    echo - Don't write inline documentation
    echo - After every new feature update the /docs folder
    echo - For each new feature implement a unit test in the tests folder
    echo - Use xUnit as the test framework
    echo - Minimal API endpoints go in `Program.cs` or separate extension files under `Endpoints/`
    echo - Blazor components go under `Components/` ^(or `Pages/`/`Shared/`^)
    echo - Before writing code, always plan first
)

> "%dest%\.opencode\.gitignore" (
    echo node_modules
    echo package.json
    echo package-lock.json
    echo bun.lock
    echo .gitignore
)

> "%dest%\.opencode\opencode.json" (
    echo {
    echo   "$schema": "https://opencode.ai/config.json",
    echo   "mcp": {
    echo     "context7": {
    echo       "type": "remote",
    echo       "url": "https://mcp.context7.com/mcp",
    echo       "enabled": true
    echo     }
    echo   }
    echo }
)

> "%dest%\.opencode\agents\test-runner.md" (
    echo ---
    echo description: You have to use this agent if you want to run the testsuite or if the user asks you to run unit tests.
    echo mode: subagent
    echo model: opencode/qwen3.6-plus-free
    echo temperature: 0.1
    echo tools:
    echo     questions: false
    echo permissions:
    echo     skill:
    echo         "*": deny
    echo     task:
    echo         "*": deny
    echo         "explore": allow
    echo     bash:
    echo         "*": allow
    echo ---
    echo.
    echo **Instructions**
    echo 1. Run unit tests with "dotnet test"
    echo 2. Try to fix potential issues
    echo 3. If all the tests pass output an instruction for the agent that invoked you to say that they all passed
    echo 4. If after a certain amount of attempts you can't fix the issues then just give back an instruction of everything you tried so a next agent can try again, or the human can try
)

> "%dest%\.opencode\agents\review.md" (
    echo ---
    echo description: Reviews code for quality and best practices
    echo mode: primary
    echo model: opencode/big-pickle
    echo temperature: 0.1
    echo tools:
    echo     write: false
    echo     edit: false
    echo     bash: true
    echo permissions:
    echo     skill:
    echo         "*": deny
    echo     task:
    echo         "*": deny
    echo         "explore": allow
    echo     bash:
    echo         "*": deny
    echo         "git diff*": allow
    echo         "git log*": allow
    echo         "git fetch*": deny
    echo         "git status*": allow
    echo ---     
    echo.
    echo You are in code review mode.
    echo Focus on:
    echo - Code quality and best practices
    echo - Potential bugs and edge cases
    echo - Performance implications
    echo - Directory structure is good
    echo - If instructions of the AGENTS.md are respected
    echo.
    echo Restrictins:
    echo - dont review the .opencode
    echo.
    echo Output your review in a formatted markdown with fields:
    echo - name of issue
    echo - priority of issue
    echo - pontential fix
)

> "%dest%\.opencode\command\create_pr.md" (
    echo ---
    echo description: Commits and Creates a PR of the current branch
    echo agent: build
    echo subtask: true
    echo ---
    echo.
    echo Create a git commit and push and create PR to my main branch using the specified commands and rules
    echo.
    echo **Instructions**
    echo.
    echo 1. Create a commit holding all uncommited code, with the parameters:
    echo.
    echo - **m** a simple message about what is being commited
    echo.
    echo **CLI command syntax**
    echo ```
    echo git commit -m ^<FILL_IN_ACCORDINGLY^>
    echo ```
    echo.
    echo 2. Inspect with a git diff everything that has changed on this branch in comparisson with the main branch
    echo.
    echo 3. Create a PR with the following params:
    echo.
    echo - **head** the current branch name. `git branch --show-current`
    echo - **title** short and appropriate title based on the changes of the PR
    echo - **body** changes in short bullet points listed. Nothing else
    echo.
    echo **CLI command syntax**
    echo ```
    echo gh pr create --base main --head ^<FILL_IN_ACCORDINGLY^> --title "<FILL_IN_ACCORDINGLY>" --body "<FILL_IN_ACCORDINGLY>"
    echo ```
    echo.
    echo If gh commands are not installed return the message "gh command not installed!"
)

> "%dest%\.opencode\skills\build-project\SKILL.md" (
    echo ---
    echo name: build-project
    echo description: Build the current project
    echo ---
    echo.
    echo **Instructions**
    echo.
    echo 1. Use the following script to build the project:
    echo **CLI command**
    echo ```
    echo .opencode/skills/build-project/scripts/build.cmd
    echo ```
    echo.
    echo 2. If the project build has errors, you have to try to fix them.
    echo If there are no errors, say "Project built successfully".
)

> "%dest%\.opencode\skills\build-project\scripts\build.cmd" (
    echo dotnet build
)

> "%dest%\.opencode\skills\second-opinion\SKILL.md" (
    echo ---
    echo name: second-opinion
    echo description: Get a second opinion on code from another agent/model
    echo ---
    echo.
    echo **Instructions**:
    echo.
    echo 1. Come up with a clearly defined and worded request for another AI agent on something you want advice or help with.
    echo.
    echo 2. Use the following cli command to ask for advice / request the another AI agent.
    echo ```
    echo opencode run "<REQUEST>" -m opencode/ring-2.6-1t-free
    echo ```
    echo.
    echo **Parameters**:
    echo - `^<REQUEST^>`: Replace this with your request for another AI agent.
    echo.
    echo 3. Potentially wait a while while the model is processing your request and generating a response.
    echo.
    echo 4. Communicate concisely to me what the other model thought and how you think about it
)

echo Done. Workflow files created in "%dest%"
endlocal
