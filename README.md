# skills

Personal [GitHub Copilot CLI](https://docs.github.com/copilot/concepts/agents/about-copilot-cli) skills by [Michael Astrauckas](https://github.com/mastrauckas).

A skill is a `SKILL.md` file that tells Copilot how to perform a specialized task — scaffolding a project, debugging a workflow, or following a set of conventions. Copilot loads the skill automatically when your prompt matches, or you can invoke it directly by name.

## Available Skills

| Skill | Description |
|-------|-------------|
| [dotnet-minimal-api](./dotnet-minimal-api/SKILL.md) | Scaffold a production-ready .NET Minimal API project following established patterns for structure, logging, health checks, rate limiting, and testing. |

## Setup

Clone this repo into your Copilot CLI skills directory:

```powershell
git clone git@github.com:mastrauckas/skills.git "$HOME\.copilot\skills"
```

For Claude Code, clone it into the Claude skills directory as well:

```powershell
git clone git@github.com:mastrauckas/skills.git "$HOME\.claude\skills"
```

Then reload skills without restarting:

```
/skills reload
```

## Usage

Copilot selects a skill automatically when your prompt matches its description. To invoke one directly, name it in your prompt:

```
Use the /dotnet-minimal-api skill to scaffold a new orders API
```

## Adding a New Skill

1. Create a directory: `mkdir my-skill-name`
2. Add `my-skill-name/SKILL.md` with YAML frontmatter and instructions
3. Commit and push
4. Run `/skills reload` in Copilot CLI

See the [GitHub Copilot CLI skills documentation](https://docs.github.com/copilot/how-tos/copilot-cli/customize-copilot/create-skills) for the full `SKILL.md` format reference.

## License

MIT — see [LICENSE](./LICENSE).
