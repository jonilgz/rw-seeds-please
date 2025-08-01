# 🤝 Contributing Guide

Thanks for your interest in contributing to this project! To keep our workflow clean and organized, we follow a branching strategy based on Git Flow, with some naming conventions that **must be followed** when submitting changes.

## 🌳 Branch Structure

- `master`: The main branch. Contains production-ready code only.
- `develop`: The development branch. All new features are integrated here first.
- `release/*`: Prepares the next production release.
- `hotfix/*`: Emergency fixes applied directly to production.
- `feature/*`: New features under development.
- `bugfix/*`: Bug fixes made during a release cycle.

---

## 📦 Branch Types & Workflow

| Branch Type       | From          | Into           | Purpose |
|-------------------|---------------|----------------|---------|
| `hotfix/xxx`      | `master`      | `master`       | Urgent fix for production |
| `release/x.x.x`   | `develop`     | `master`       | Prepares a version release |
| `feature/xxx`     | `develop`     | `develop`      | New feature in progress |
| `bugfix/xxx`      | `release/x.x.x` | `release/x.x.x` | Bugfix during the release phase |

---

## 🚦 Pull Request Rules

- **All changes must go through a Pull Request (PR).** No direct commits to protected branches.
- The source branch of the PR **must follow the correct naming convention** (see table above).
- A valid PR must:
  - Pass any applicable tests or checks.
  - Be reviewed and approved by at least one team member.
  - Include a clear description of the changes and the reasoning behind them.
- **Do not mix multiple logical changes** in a single PR (e.g., a feature and a bugfix together).

---

## 🧼 Best Practices

- Frequently rebase your branch with its target base (`develop`, `release`, etc.) to avoid conflicts and keep history clean.
- Write atomic, descriptive commits.
- Delete merged remote branches when they are no longer needed.
- When in doubt, open an issue or ask before submitting a PR.

---

Thanks for helping keep the project clean and organized
