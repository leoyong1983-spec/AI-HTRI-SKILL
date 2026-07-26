# HTRI Xist 9.0 Automation Server Reference

## Scope

This reference records locally verified behavior for HTRI Xchanger Suite 9.0.21350.0401 on Windows. Re-verify field keys and units for other releases.

## Runtime identity

- GUI executable: `HtriGui.exe`
- COM ProgID: `HtriCalc.HeatExchangerNetwork`
- Primary control path: official HTRI Automation Server
- Verified module: Xist shell-and-tube cases
- Verified template: installed `Samples\Xist_Sample.htri`

Do not bundle HTRI binaries, samples, or proprietary type libraries with the skill.

## Minimum control sequence

```text
Create HtriCalc.HeatExchangerNetwork in an STA process
EnableReturnDefault(1)
EnableSaveOutputData(1)
OpenFile(source-copy)
GetHeatTransferUnitCount()
GetMaterialStreamCount()
GetHeatTransferUnit(0)
CheckNetworkForRun()
GetRunCheckMessageList()
SetRunDialog(0, 0, description, 0)
Run(0, 0)
GetMessageList()
SaveFile(new-output-path)
```

Verified success values:

- `OpenFile`: nonzero means opened.
- `CheckNetworkForRun`: `0` means the input check passed.
- `Run`: `3` means the run completed successfully.
- Successful messages include `Run Completed` and `Solution Reached`.

Always capture severity, type, number, and text for every check/run message.

## Internal unit traps

The Automation Server can expose internal US customary values even when the GUI/report is SI.

| Quantity | Verified raw unit | SI conversion |
|---|---|---|
| Process flow | 1000 lb/h | multiply by 453.59237 for kg/h |
| Temperature | deg F | `(F-32)*5/9` for deg C |
| Material-stream thermodynamic pressure | psia | multiply by 6.894757293 for kPa(a) |
| Shell/tube design pressure keys | object-specific; verified examples expose psig | query the key UOM, then convert explicitly |
| Pressure drop | psi | multiply by 6.894757293 for kPa |
| Duty | MM Btu/h | multiply by 0.29307107 for MW |
| Area | ft2 | multiply by 0.09290304 for m2 |
| EMTD | deg F difference | multiply by 5/9 for K |
| U or film coefficient | Btu/(h.ft2.F) | multiply by 5.67826334 for W/(m2.K) |

Never write a flow as lb/h when the field expects 1000 lb/h. Pressure units are object-specific: query `GetUOMString(key)` before every write. Always convert gauge pressure to absolute before writing a thermodynamic state; do not apply that absolute value blindly to a design field whose UOM is psig.

## Verified Xist output keys

These numeric keys were verified on Xist 9.0 only.

| Object | Key | Meaning |
|---|---:|---|
| Exchanger performance | 1582 | heat duty |
| Exchanger performance | 1583 | gross area |
| Exchanger performance | 1584 | effective area |
| Exchanger performance | 1585 | EMTD |
| Exchanger performance | 1587 | actual overall U |
| Exchanger performance | 1588 | overdesign percent |
| Shell performance | 2808 | shell-side film coefficient |
| Tube performance | 3378 | tube-side film coefficient |
| Tube performance | 3377 | tube-side pressure drop |

Verified geometry mapping:

- `TType_OutsideDiameter = 3509`
- `TType_HeatedLength = 3504`

Do not infer adjacent keys. Probe the installed type library and verify the saved input report before accepting any write.

## Process and property rules

- Store inlet pressure in the inlet material stream overall thermodynamic state; changing only a visible process-condition object may leave the property state inconsistent.
- Record whether pressure is absolute or gauge at every boundary.
- Preserve stream side assignment, flow specification mode, vapor fraction, and phase property method.
- Treat `-1E24` doubles and `-32000` shorts as missing/default sentinels, not numerical results.
- HTRI 9.0 property-curve writes in the verified workflow used 30 points per curve. Longer writes were truncated in testing. Re-probe before using a different count.
- Preserve the independent property source. For DWSIM, record version, package, composition, pressure grid, temperature/enthalpy grid, and exported CSV hash.

## Safe field-change procedure

1. Open a copied case and export its input report.
2. Read the current value and unit.
3. Change one field.
4. Save under a new name.
5. Reopen in HTRI and export the input report again.
6. Confirm the intended visible field changed and unrelated fields did not.
7. Run, save, close, reopen, and rerun.

Use GUI inspection only after the structured check. UI appearance does not prove that the Automation Server wrote the intended thermodynamic state.
