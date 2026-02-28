# skills

Personal Copilot CLI skills for [@mastrauckas](https://github.com/mastrauckas).

## Setup

Clone this repo directly into your Copilot skills directory:

```powershell
git clone git@github.com:mastrauckas/skills.git "$HOME\.copilot\skills"
```

Then in Copilot CLI, run `/skills reload` to pick up any new skills.

## Available Skills

| Skill | Description |
|-------|-------------|
| [dotnet-minimal-api](./dotnet-minimal-api/SKILL.md) | Best practices for creating .NET Minimal API projects |

## Usage

Invoke a skill explicitly in your prompt:

```
Use the /dotnet-minimal-api skill to scaffold a new products API
```

Or just describe the task and Copilot will load the skill automatically when relevant.

## Adding a New Skill

1. Create a new directory: `mkdir skill-name`
2. Create `skill-name/SKILL.md` with YAML frontmatter + instructions
3. Commit and push
4. Run `/skills reload` in Copilot CLI
