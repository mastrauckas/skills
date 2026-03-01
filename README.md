# skills

A collection of AI coding agent skills by [Michael Astrauckas](https://github.com/mastrauckas).

A skill is a `SKILL.md` file that tells Copilot how to perform a specialized task — scaffolding a
project, debugging a workflow, or following a set of conventions. Copilot loads the skill
automatically when your prompt matches, or you can invoke it directly by name.

## Available Skills

| Skill                                               | Description                                              |
| --------------------------------------------------- | -------------------------------------------------------- |
| [dotnet-minimal-api](./dotnet-minimal-api/SKILL.md) | Scaffold a production-ready .NET 10 Minimal API project. |

## Setup

Clone this repo into your Copilot CLI skills directory:

```powershell
git clone git@github.com:mastrauckas/ai.git "$HOME\.copilot\skills"
```

For Claude Code, clone it into the Claude skills directory as well:

```powershell
git clone git@github.com:mastrauckas/ai.git "$HOME\.claude\skills"
```

Then reload skills in Copilot CLI without restarting:

```
/skills reload
```

> **Note:** `/skills reload` is a GitHub Copilot CLI command. Claude Code picks up skills
> automatically on restart.

## Usage

Copilot selects a skill automatically when your prompt matches its description. To invoke one
directly, name it in your prompt:

```
Use the /dotnet-minimal-api skill to scaffold a new orders API
```

## Adding a New Skill

1. Create a directory: `mkdir my-skill-name`
2. Add `my-skill-name/SKILL.md` with YAML frontmatter and instructions
3. Commit and push
4. Run `/skills reload` in Copilot CLI

See the
[GitHub Copilot CLI skills documentation](https://docs.github.com/copilot/how-tos/copilot-cli/customize-copilot/create-skills)
for the full `SKILL.md` format reference.

## License

MIT — see [LICENSE](./LICENSE).
