// Fluent UI 2 foundation-spec generator.
//
// Reads the resolved @fluentui/tokens theme (webLightTheme / webDarkTheme /
// typographyStyles) plus the raw brand + grey ramps, and emits two markdown specs
// into docs/spec, in the same table style as the MD3 specs:
//   docs/spec/_pallete/fluentui2-spec.md    - all color alias tokens (light/dark) + ramps
//   docs/spec/_foundation/fluentui2-spec.md - type / spacing / radius / stroke / shadow / motion
//
// NOTE: only the FOUNDATION (global + alias) layer is generated here. The per-component
// specs (docs/spec/<comp>/fluentui2-spec.md) are AUTHORED from each component's
// @fluentui/react-* `use*Styles.styles.ts` source, because Fluent keeps per-component
// styling in code, not in a declarative token table.
//
// Usage:  npm install  &&  npm run generate
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { createRequire } from 'node:module';

const require = createRequire(import.meta.url);
const here = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(here, '..', '..');
const SPEC_ROOT = path.join(REPO_ROOT, 'docs', 'spec');
const OUT = 'fluentui2-spec.md';

const tokens = require('@fluentui/tokens');
const version = require('@fluentui/tokens/package.json').version;
const L = tokens.webLightTheme;
const D = tokens.webDarkTheme;
const TS = tokens.typographyStyles;

// ---- raw ramps (from the compiled global source; values are static literals) ----
const pkgGlobalDir = path.join(path.dirname(require.resolve('@fluentui/tokens/package.json')),
  'lib-commonjs', 'global');
function extractRamp(file, name) {
  const src = fs.readFileSync(path.join(pkgGlobalDir, file), 'utf8');
  const block = src.match(new RegExp(`const ${name} = \\{([\\s\\S]*?)\\};`))[1];
  const ramp = {};
  for (const m of block.matchAll(/['"`]?(\w+)['"`]?\s*:\s*['"`]([^'"`]+)['"`]/g))
    ramp[m[1]] = m[2];
  return ramp;
}
const brandWeb = extractRamp('brandColors.js', 'brandWeb');
const grey = extractRamp('colors.js', 'grey');

// ---- helpers ----
const rows = (header, arr) =>
  [`| ${header.join(' | ')} |`,
   `|${header.map(() => '---').join('|')}|`,
   ...arr.map(r => `| ${r.join(' | ')} |`)].join('\n');

function resolveTypo(style) {
  const deref = v => {
    const m = /^var\(--(\w+)\)$/.exec(v);
    return m ? (L[m[1]] ?? v) : v;
  };
  return {
    family: deref(style.fontFamily),
    size: deref(style.fontSize),
    weight: deref(style.fontWeight),
    line: deref(style.lineHeight),
  };
}

// ---- color grouping (semantic alias roles) ----
const colorKeys = Object.keys(L).filter(k => k.startsWith('color'));
const GROUPS = [
  ['Neutral Foreground', k => /^colorNeutralForeground/.test(k)],
  ['Neutral Background', k => /^colorNeutralBackground/.test(k)],
  ['Neutral Stroke', k => /^colorNeutralStroke/.test(k)],
  ['Brand', k => /^colorBrand/.test(k)],
  ['Compound Brand', k => /^colorCompoundBrand/.test(k)],
  ['Subtle / Transparent', k => /^color(Subtle|Transparent)/.test(k)],
  ['Status', k => /^colorStatus/.test(k)],
];
function groupOf(k) {
  for (const [name, test] of GROUPS) if (test(k)) return name;
  if (/^colorPalette/.test(k)) return 'Shared palette';
  return 'Other';
}

// =====================================================================
// FILE 1 - palette (color)
// =====================================================================
function buildPalette() {
  const out = [];
  out.push('# Fluent UI 2 - палитра', '');
  out.push('Разрешённые семантические alias-токены цвета из `@fluentui/tokens` '
    + `(webLightTheme / webDarkTheme, версия ${version}). `
    + 'В отличие от MD3, у Fluent НЕТ per-component цветовых токенов - компоненты '
    + 'потребляют эти alias-роли напрямую.', '');

  const order = ['Neutral Foreground', 'Neutral Background', 'Neutral Stroke',
    'Brand', 'Compound Brand', 'Subtle / Transparent', 'Status', 'Other'];
  const label = { 'Other': 'Прочие нейтральные роли (card / overlay / focus / stencil / shadow)' };
  const byGroup = {};
  for (const k of colorKeys) (byGroup[groupOf(k)] ||= []).push(k);

  out.push('## Семантические alias-роли (light / dark)', '');
  for (const g of order) {
    const ks = byGroup[g]; if (!ks || !ks.length) continue;
    out.push(`### ${label[g] ?? g}`, '');
    out.push(rows(['Токен', 'Light', 'Dark'], ks.map(k => [`\`${k}\``, L[k], D[k]])), '');
  }

  if (byGroup['Shared palette']?.length) {
    out.push('## Общие цветовые рампы (shared palette, light / dark)', '');
    out.push(rows(['Токен', 'Light', 'Dark'],
      byGroup['Shared palette'].map(k => [`\`${k}\``, L[k], D[k]])), '');
  }

  out.push('## Тональные рампы (легенда)', '');
  out.push('### grey (глобальная нейтральная рампа)', '');
  out.push(rows(['Тон', 'Hex'], Object.entries(grey).map(([t, v]) => [`grey${t}`, v])), '');
  out.push('### brandWeb (акцентная рампа)', '');
  out.push(rows(['Тон', 'Hex'], Object.entries(brandWeb).map(([t, v]) => [`brand${t}`, v])), '');

  return out.join('\n') + '\n';
}

// =====================================================================
// FILE 2 - foundation (non-color)
// =====================================================================
function buildFoundation() {
  const out = [];
  out.push('# Fluent UI 2 - foundation', '');
  out.push('Не-цветовые global/alias токены из `@fluentui/tokens` '
    + `(версия ${version}). `
    + 'Значения одинаковы для light и dark (различаются только цвета).', '');

  out.push('## Типографика - композитные стили', '');
  out.push(rows(['Стиль', 'Font family', 'Weight', 'Size', 'Line-height'],
    Object.entries(TS).map(([name, s]) => {
      const r = resolveTypo(s);
      return [name, r.family.replace(/'/g, ''), r.weight, r.size, r.line];
    })), '');

  const pick = re => Object.keys(L).filter(k => re.test(k));
  const table = (title, re) => {
    out.push(`## ${title}`, '');
    out.push(rows(['Токен', 'Значение'], pick(re).map(k => [`\`${k}\``, String(L[k])])), '');
  };
  table('Типографика - размеры шрифта', /^fontSize/);
  table('Типографика - высота строки', /^lineHeight/);
  table('Типографика - начертание', /^fontWeight/);
  table('Типографика - семейства', /^fontFamily/);
  table('Отступы - горизонтальные', /^spacingHorizontal/);
  table('Отступы - вертикальные', /^spacingVertical/);
  table('Скругления (border radius)', /^borderRadius/);
  table('Толщина обводки (stroke width)', /^strokeWidth/);
  table('Тени (shadow)', /^shadow/);
  table('Движение - кривые (curves)', /^curve/);
  table('Движение - длительности (durations)', /^duration/);

  return out.join('\n') + '\n';
}

// ---- write ----
function write(folder, content) {
  const dir = path.join(SPEC_ROOT, folder);
  fs.mkdirSync(dir, { recursive: true });
  const file = path.join(dir, OUT);
  fs.writeFileSync(file, content);
  console.log(`wrote ${path.relative(REPO_ROOT, file)} (${content.split('\n').length} lines)`);
}
write('_pallete', buildPalette());
write('_foundation', buildFoundation());
console.log(`done (@fluentui/tokens ${version}).`);
