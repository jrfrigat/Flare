// Build-time generator: reads Material Symbols SVG files and emits a MaterialDesign3Icons catalog of
// FlareSvgIcon values for the Flare.Icons.MaterialDesign3.Svg package. Material Symbols are Apache-2.0.
//
// Material Symbols is a variable icon FONT; Google also publishes every symbol as a static SVG. This tool
// consumes those SVGs (it does NOT parse the font), so the output is Google's own artwork, not a lossy
// outline conversion. Note the symbols are authored on a 960 grid with a flipped origin
// (viewBox="0 -960 960 960"), which is captured per icon and set on FlareSvgIcon.ViewBox.
//
// SOURCE - get it once on a networked machine, then this tool runs fully offline:
//   npm i @material-symbols/svg-400        (marella; also svg-100..700 for other weights)
//     -> node_modules/@material-symbols/svg-400/{outlined,rounded,sharp}/<name>.svg  (+ <name>-fill.svg)
//   or clone github.com/google/material-design-icons and point MSYM_SRC at a folder of <name>.svg files.
//
// LAYOUT auto-detect from MSYM_SRC:
//   - if MSYM_SRC/<style> is a directory        -> use MSYM_SRC/<style>   (the marella package layout)
//   - else                                      -> use MSYM_SRC           (a flat folder of *.svg)
// A file named "<name>-fill.svg" is the filled (FILL 1) variant; "<name>.svg" is regular (FILL 0).
//
// CONFIG (env or edit the consts):
//   MSYM_SRC    = path to the svg root (REQUIRED)
//   MSYM_STYLE  = rounded | outlined | sharp     (default rounded - the Flare/Gallery default family)
//
// RUN:  MSYM_SRC=node_modules/@material-symbols/svg-400 node tools/MaterialSymbolsGen/gen.mjs
//
// After first run: add src/Flare.Icons.MaterialDesign3.Svg to Flare.slnx + a NOTICE entry, then build.

import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const HERE = path.dirname(fileURLToPath(import.meta.url));
const REPO = path.resolve(HERE, '..', '..');

const STYLE = (process.env.MSYM_STYLE || 'rounded').toLowerCase();
if (!['rounded', 'outlined', 'sharp'].includes(STYLE)) {
  console.error(`MSYM_STYLE must be rounded|outlined|sharp, got "${STYLE}"`);
  process.exit(1);
}
const SRC = process.env.MSYM_SRC;
if (!SRC) {
  console.error('Set MSYM_SRC to the Material Symbols svg root (see the header of this file).');
  process.exit(1);
}

// Resolve the actual directory of *.svg files (marella <style> subfolder, or a flat folder).
const styled = path.join(SRC, STYLE);
const dir = fs.existsSync(styled) && fs.statSync(styled).isDirectory() ? styled : SRC;
if (!fs.existsSync(dir)) {
  console.error(`Source directory not found: ${dir}`);
  process.exit(1);
}

// A C# identifier from a snake/kebab file name; a digit-leading name (e.g. "3d_rotation", "10k") is
// prefixed with '_' so it stays a legal identifier.
function toIdent(base) {
  const id = base
    .split(/[_\-\s]+/)
    .filter(Boolean)
    .map(p => p.charAt(0).toUpperCase() + p.slice(1))
    .join('');
  return /^[0-9]/.test(id) ? '_' + id : id;
}

// Pull the viewBox and the inner markup (everything inside <svg>...</svg>) out of one SVG file.
function parseSvg(file) {
  const text = fs.readFileSync(file, 'utf8');
  const vb = (text.match(/viewBox="([^"]*)"/i) || [])[1] || '0 -960 960 960';
  const inner = (text.match(/<svg[^>]*>([\s\S]*?)<\/svg>/i) || [])[1] || '';
  return { vb, inner: inner.trim() };
}

// name -> { regular?: {vb,inner}, filled?: {vb,inner} }
const icons = new Map();
for (const f of fs.readdirSync(dir)) {
  if (!f.endsWith('.svg')) continue;
  const isFill = f.endsWith('-fill.svg');
  const base = f.replace(/-fill\.svg$/, '').replace(/\.svg$/, '');
  const parsed = parseSvg(path.join(dir, f));
  if (!parsed.inner || parsed.inner.includes('"""')) continue; // skip empty / raw-string-hostile art
  const entry = icons.get(base) || {};
  entry[isFill ? 'filled' : 'regular'] = parsed;
  icons.set(base, entry);
}

// Emit one nested class (Regular/Filled) from base-name -> parsed, de-duping identifier collisions.
function emitClass(cls, pick) {
  const lines = [];
  lines.push(`    /// <summary>${cls} (FILL ${cls === 'Filled' ? 1 : 0}) Material Symbols (${STYLE}).</summary>`);
  lines.push(`    public static class ${cls}`);
  lines.push('    {');
  const seen = new Set();
  let count = 0;
  for (const base of [...icons.keys()].sort()) {
    const art = pick(icons.get(base));
    if (!art) continue;
    const id = toIdent(base);
    if (seen.has(id)) { console.warn(`skip dup ident ${id} (${base})`); continue; }
    seen.add(id);
    lines.push(`        /// <summary>The Material Symbols <c>${base}</c> (${STYLE}, ${cls.toLowerCase()}) icon.</summary>`);
    // Expression-bodied (computed) so a full catalog does not build a pathological static constructor.
    lines.push(`        public static FlareSvgIcon ${id} => new() { Data = """${art.inner}""", ViewBox = "${art.vb}" };`);
    count++;
  }
  lines.push('    }');
  return { text: lines.join('\n'), count };
}

const regular = emitClass('Regular', e => e.regular);
const filled = emitClass('Filled', e => e.filled);

let out = '';
out += '// <auto-generated />\n';
out += `// Generated by tools/MaterialSymbolsGen from Material Symbols (${STYLE}), Apache License 2.0.\n`;
out += '// Re-run the generator to refresh/extend. See the repository NOTICE.\n\n';
out += 'namespace Flare.Icons;\n\n';
out += '/// <summary>\n';
out += `/// The Material Design 3 (Material Symbols, ${STYLE}) set as ready <see cref="FlareSvgIcon"/> values -\n`;
out += '/// inline SVG, no icon font. Regular (FILL 0) and Filled (FILL 1) variants. Use anywhere a\n';
out += '/// <see cref="FlareIcon"/> is accepted, e.g. <c>&lt;FlareIconButton Icon="@MaterialDesign3Icons.Regular.Home" /&gt;</c>.\n';
out += '/// </summary>\n';
out += 'public static class MaterialDesign3Icons\n';
out += '{\n';
out += regular.text + '\n\n';
out += filled.text + '\n';
out += '}\n';

const outDir = path.join(REPO, 'src', 'Flare.Icons.MaterialDesign3.Svg');
fs.mkdirSync(outDir, { recursive: true });
const outPath = path.join(outDir, 'MaterialDesign3Icons.g.cs');
fs.writeFileSync(outPath, out, 'utf8');

console.log(`style: ${STYLE}, source: ${dir}`);
console.log(`Regular: ${regular.count}, Filled: ${filled.count}`);
console.log(`Wrote ${outPath}`);
