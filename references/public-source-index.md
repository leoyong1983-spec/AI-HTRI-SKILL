# Public Source Index

Reviewed 2026-07-27. This index records public sources only; it does not include, copy, or redistribute HTRI member material, binaries, sample cases, type libraries, or proprietary calculation files.

| Source | Published/version | Access and license status | Grade and relation | Decision and boundary |
|---|---|---|---|---|
| [HTRI Design Manual](https://www.htri.net/design-manual) | Not stated on the public landing page | Public landing page; full manual content was not retrieved | **A**; official HTRI description of the calculation-reference scope | Retain as a public orientation source for pressure drop, heat transfer, condensation, boiling, two-phase flow, fouling, vibration, and equipment guidance. Do not infer equations, coefficients, applicability limits, or manual text that is not public. |
| [TechTip: Using the HTRI Automation Server](https://www.htri.net/webinars/techtip/using-the-htri-automation-server) | 2014-08-27 | Public webinar description; no webinar asset saved | **A**; official, direct evidence for the Automation Server workflow | Supports the high-level existing-case workflow: load, modify, run, retrieve outputs, and save. It does **not** prove a particular COM ProgID, field key, unit, return code, or safe write behavior on the installed version; those remain local-runtime evidence only. |
| [TechTip: Using the HTRI Parametric Study Tool](https://www.htri.net/webinars/techtip/using-the-htri-parametric-study-tool) | Date not reliably extracted from the public page | Public webinar description; Knowledge Base asset not retrieved or saved | **A**; official, direct evidence for template-based parameter sweeps | Adopt the audit rule for parametric series: preserve a frozen baseline and maintain result-to-case/parameter traceability. The source does **not** validate any particular field key, Excel/OLE behavior, or result on the installed version. |
| [Xchanger Suite overview and version 9.4 changes](https://www.htri.net/software/xchanger-suite) | Xchanger Suite 9.4; page date not stated | Public product page; no asset saved | **A** for version 9.4 release information; adjacent to the locally verified 9.0 workflow | Adopt the general version-regression rule in `SKILL.md`: published changes include heat-transfer and property options, so a model must be benchmarked after a version change. Do not apply any 9.4-specific method to 9.0 without exact-version evidence. |

## Rejected or deferred evidence

- General heat-exchanger papers or studies that merely report an HTRI comparison are not evidence for HTRI Automation Server fields, calculation internals, or target-version runtime behavior. No such paper was adopted in this review.
- Public vendor descriptions do not replace the licensed Design Manual, an installed-version test, an accepted project model, or the required independent reopen-and-rerun evidence package.
