---
description: Commits and Creates a PR of the current branch
agent: build
subtask: true
---

Create a git commit and push and create PR to my main branch using the specified commands and rules

**Instructions**

1. Create a commit holding all uncommited code, with the parameters:

- **m** a simple message about what is being commited

**CLI command syntax**
```
git commit -m <FILL_IN_ACCORDINGLY>
```

2. Inspect with a git diff everything that has changed on this branch in comparisson with the main branch

3. Create a PR with the following params:

- **head** the current branch name. `git branch --show-current`
- **title** short and appropriate title based on the changes of the PR
- **body** changes in short bullet points listed. Nothing else

**CLI command syntax**
```
gh pr create --base main --head <FILL_IN_ACCORDINGLY> --title "<FILL_IN_ACCORDINGLY>" --body "<FILL_IN_ACCORDINGLY>"
```

If gh commands are not installed return the message "gh command not installed!"
