# AI-HTRI-SKILL

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

An independent, auditable Codex skill for controlling existing HTRI Xchanger Suite cases on Windows through the official Automation Server.

The project favors controlled case takeover, minimal edits, deterministic reruns, structured exports, reopen verification, and explicit engineering acceptance boundaries. It is intended for engineers who already have a valid HTRI installation, license, and case files.

## Capabilities

- Detect an installed HTRI environment and optionally probe the Automation Server COM interface.
- Audit an existing `.htri` case without overwriting the source.
- Rerun a bounded Xist Rating workflow and export traceable JSON/CSV/log evidence.
- Require saved-copy reopen verification and an independent duty balance before acceptance.
- Separate thermal convergence from property, geometry, warning, vibration, operability, and mechanical review.
- Document bounded equivalent-Rating workflows for equipment without a native HTRI geometry model.

## Safety and scope

This repository does **not** include HTRI software, licenses, binaries, type libraries, member-only documentation, proprietary sample cases, vendor files, or project data. It does not bypass HTRI access controls.

The included Automation Server reference records locally verified behavior for Xist 9.0.21350.0401. Field keys, units, return codes, methods, and report behavior must be reverified for other releases. A converged run is not evidence that the physical exchanger is correctly represented or fit for service.

## Requirements

- Windows
- A licensed HTRI Xchanger Suite installation
- PowerShell
- A C# compiler available through the supported Windows/.NET installation
- An existing runnable case or an authorized official sample for the target module

## Install as a Codex skill

```powershell
git clone https://github.com/leoyong1983-spec/AI-HTRI-SKILL.git `
  "$env:USERPROFILE\.codex\skills\ai-htri-skill"
```

Restart Codex after installation, then invoke the skill explicitly when needed:

```text
Use $ai-htri-skill to audit an existing Xist case without modifying the source file.
```

## Quick environment check

From the repository root:

```powershell
& ".\scripts\Test-HtriEnvironment.ps1" -ProbeCom
```

Do not run a case until the installed version, module, license availability, source path, pressure basis, units, geometry revision, and acceptance criteria have been confirmed.

## Repository layout

- `SKILL.md` — Codex workflow and engineering guardrails.
- `agents/openai.yaml` — Codex UI metadata.
- `scripts/` — environment check, read-only audit, and controlled rerun wrappers.
- `references/` — versioned Automation Server evidence, reporting rules, model boundaries, and public-source provenance.
- `terminology_inbox.md` — unapproved candidate rules that still require verification or engineering confirmation.

## License and trademarks

The original content and code in this repository are released under the [MIT License](LICENSE).

HTRI, Xchanger Suite, Xist, Xace, Xvib, and other third-party names belong to their respective owners. This project is independent and is not endorsed by or affiliated with Heat Transfer Research, Inc. Links and source summaries do not grant redistribution rights to third-party material.
