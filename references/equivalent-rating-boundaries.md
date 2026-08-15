# Equivalent Rating Boundaries

## Select the model class first

Use a native HTRI module and geometry whenever the equipment is directly represented. Use an equivalent model only when the target equipment has no native geometry and the user accepts a bounded thermal approximation.

For a water-bath wound-coil vaporizer, an Xist shell-and-tube surrogate does not reproduce the real bath, coil curvature, headers, circuit distribution, vapor disengagement, or two-phase pressure drop. It can still test whether a stated area and assumed heat-transfer coefficients are thermally sufficient.

## Define the exact question

Acceptable equivalent-Rating questions include:

- What overall U is required at the design duty and area?
- Is the stated area sufficient for a bounded U range?
- What is the sensitivity to hot-side film coefficient, tube-side coefficient, fouling, effective area, and utility flow?
- Is utility duty consistent with the required process enthalpy rise?

Do not answer from a surrogate:

- guaranteed two-phase pressure drop;
- stable parallel-circuit distribution;
- dryout, slugging, vibration, or mechanical integrity;
- nozzle/header losses;
- manufacturing acceptance without a vendor guarantee or performance test.

## Distinguish calculated and imposed coefficients

For every side, classify the film coefficient method:

1. calculated by a native correlation on representative geometry;
2. imported from a vendor or test;
3. manually imposed;
4. multiplied by an engineering correction.

If either side uses item 3 or 4, describe the output U as conditional. Never call it a predicted or guaranteed equipment U.

Use the thermal-resistance form to explain the result:

```text
1/Uo = 1/ho + Rfo + wall resistance + area-ratio*(Rfi + 1/hi)
```

State the area basis (`inside` or `outside`) and keep it consistent.

## Mandatory sensitivity set

At minimum vary:

- hot-side film coefficient;
- tube-side film coefficient or circuit/mass-flux assumption;
- inside and outside fouling;
- effective area fraction;
- utility flow and inlet temperature;
- process duty/property uncertainty.

Do not vary only the hot-side film when the tube-side coefficient is controlled by an uncertain parallel-circuit assumption.

Report:

- required U at the design point;
- required area for each U scenario;
- area including the specified design margin;
- utility-limited duty;
- the point at which area or utility becomes controlling.

## Water-bath wound-coil warning signs

Escalate to vendor confirmation or testing when any of these apply:

- circuit count is inferred rather than drawing-confirmed;
- circuit lengths differ materially;
- a flow multiplier is used to represent fewer real circuits than virtual tubes;
- two-phase distribution is not modeled;
- the model predicts a very high tube-side film coefficient or pressure drop;
- effective area assumes every descending/ascending section remains wetted and active;
- the HTRI geometry is TEMA BEU/BEM while the drawing is a continuous wound coil;
- model pressure, outlet temperature, flow, tube dimensions, or area differ from the current data sheet.

In these cases, present a conservative equipment-level U range and make the vendor guarantee the minimum U, duty, outlet phase, and outlet temperature under the actual utility conditions.
