// Fluent UI v8 (Fluent 1) foundation-spec generator.
//
// Reads the resolved v8 themes and emits two markdown specs into docs/spec, in the
// same table style as the Fluent 2 specs:
//   docs/spec/_pallete/fluentui1-spec.md    - palette + semanticColors (light/dark) + color ramps
//   docs/spec/_foundation/fluentui1-spec.md - type / radius / shadow / spacing / motion
//
// Light = createTheme() from @fluentui/theme (identical to its FluentTheme export).
// Dark  = DARK_SOURCE below. v8 core ships no dark theme; the only dark theme Microsoft
// publishes for v8 is DarkTheme in @fluentui/theme-samples, so that is the reference.
//
// NOTE: only the FOUNDATION layer is generated here. Per-component specs
// (docs/spec/<comp>/fluentui1-spec.md) are AUTHORED from each component's
// @fluentui/react `*.styles.ts` source, because v8 keeps component styling in code.
//
// Usage:  npm install  &&  npm run generate
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { createRequire } from 'node:module';

// The ESM build of @fluentui/theme uses extensionless imports that Node cannot resolve,
// so the CommonJS build is loaded instead.
const require = createRequire(import.meta.url);
const here = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(here, '..', '..');
const SPEC_ROOT = path.join(REPO_ROOT, 'docs', 'spec');
const OUT = 'fluentui1-spec.md';

// Reference dark scheme: package + export. Change it here to pick another dark theme.
const DARK_SOURCE = { pkg: '@fluentui/theme-samples', exportName: 'DarkTheme' };

// These packages do not export './package.json', so the manifest is found by walking up
// from the package entry point to the folder whose package.json carries its name.
function packageDir(name, from) {
  let dir = path.dirname(require.resolve(name, from ? { paths: [from] } : undefined));
  while (true) {
    const manifest = path.join(dir, 'package.json');
    if (fs.existsSync(manifest) && JSON.parse(fs.readFileSync(manifest, 'utf8')).name === name)
      return dir;
    const parent = path.dirname(dir);
    if (parent === dir) throw new Error(`package.json of '${name}' not found.`);
    dir = parent;
  }
}
const versionOf = name =>
  JSON.parse(fs.readFileSync(path.join(packageDir(name), 'package.json'), 'utf8')).version;

const T = require('@fluentui/theme');
const themeVersion = versionOf('@fluentui/theme');
const darkPkg = require(DARK_SOURCE.pkg);
const darkVersion = versionOf(DARK_SOURCE.pkg);

// The dark theme is built by @fluentui/react's createTheme. It must be the same
// @fluentui/theme module as the light one, or the two columns come from different releases.
const reactThemePath = require.resolve('@fluentui/theme',
  { paths: [packageDir('@fluentui/react', packageDir(DARK_SOURCE.pkg))] });
if (reactThemePath !== require.resolve('@fluentui/theme'))
  throw new Error(`@fluentui/react resolves a second @fluentui/theme (${reactThemePath}); `
    + 'fix the overrides in package.json.');

const L = T.createTheme({});
const D = darkPkg[DARK_SOURCE.exportName];
if (!D || !D.palette || !D.semanticColors)
  throw new Error(`${DARK_SOURCE.pkg} has no theme export '${DARK_SOURCE.exportName}'.`);
if (!D.isInverted)
  throw new Error(`${DARK_SOURCE.pkg}.${DARK_SOURCE.exportName} is not an inverted (dark) theme.`);

// Non-color values are written once, so the dark theme must not diverge from the light one.
for (const key of ['fonts', 'effects', 'spacing'])
  if (JSON.stringify(L[key]) !== JSON.stringify(D[key]))
    throw new Error(`Light and dark themes differ in '${key}'; the foundation needs two columns.`);

// ---- helpers ----
const rows = (header, arr) =>
  [`| ${header.join(' | ')} |`,
   `|${header.map(() => '---').join('|')}|`,
   ...arr.map(r => `| ${r.join(' | ')} |`)].join('\n');
const code = k => `\`${k}\``;
const family = f => f.replace(/'/g, '');

function groupBy(keys, groups) {
  const out = new Map(groups.map(([name]) => [name, []]));
  for (const k of keys) {
    const hit = groups.find(([, test]) => test(k));
    if (!hit) throw new Error(`No group for '${k}'; extend the group list.`);
    out.get(hit[0]).push(k);
  }
  return out;
}

const PALETTE_GROUPS = [
  ['Тема (акцентная лестница)', k => /^theme/.test(k)],
  ['Нейтральная лестница', k => /^(black|white|neutral)/.test(k)],
  ['Акцент', k => k === 'accent'],
  ['Общие цвета', () => true],
];

const SEMANTIC_GROUPS = [
  ['Фон и текст страницы (body)', k => /^(body|variantBorder|defaultStateBackground)/.test(k)],
  ['Карточки', k => /^card/.test(k)],
  ['Кнопки', k => /^(button|primaryButton|accentButton)/.test(k)],
  ['Поля ввода', k => /^(input|smallInput)/.test(k)],
  ['Списки', k => /^list/.test(k)],
  ['Меню', k => /^menu/.test(k)],
  ['Ссылки', k => /^(link|actionLink)/.test(k)],
  ['Недоступное состояние', k => /^disabled/.test(k)],
  ['Фокус', k => /^focus/.test(k)],
  ['Сообщения и статусы', () => true],
];

// =====================================================================
// FILE 1 - palette (color)
// =====================================================================
function buildPalette() {
  const out = [];
  out.push('# Fluent UI v8 (Fluent 1) - палитра', '');
  out.push('Разрешенные цвета темы Fluent UI v8. Светлая схема - `createTheme()` из '
    + `\`@fluentui/theme\` ${themeVersion}; темная - \`${DARK_SOURCE.exportName}\` из `
    + `\`${DARK_SOURCE.pkg}\` ${darkVersion} (в ядре v8 темной темы нет). `
    + 'Компоненты v8 берут цвета из `semanticColors`, а те выводятся из `palette`.', '');

  out.push('## palette (light / dark)', '');
  for (const [name, ks] of groupBy(Object.keys(L.palette), PALETTE_GROUPS)) {
    if (!ks.length) continue;
    out.push(`### ${name}`, '');
    out.push(rows(['Токен', 'Light', 'Dark'], ks.map(k => [code(k), L.palette[k], D.palette[k]])), '');
  }

  out.push('## semanticColors (light / dark)', '');
  for (const [name, ks] of groupBy(Object.keys(L.semanticColors), SEMANTIC_GROUPS)) {
    if (!ks.length) continue;
    out.push(`### ${name}`, '');
    out.push(rows(['Токен', 'Light', 'Dark'],
      ks.map(k => [code(k), L.semanticColors[k], D.semanticColors[k]])), '');
  }

  const ramp = (title, obj) => {
    out.push(`### ${title}`, '');
    out.push(rows(['Токен', 'Hex'], Object.entries(obj).map(([k, v]) => [code(k), v])), '');
  };
  out.push('## Справочные рампы (одинаковы для обеих схем)', '');
  ramp('NeutralColors', T.NeutralColors);
  ramp('SharedColors', T.SharedColors);
  ramp('CommunicationColors', T.CommunicationColors);

  return out.join('\n') + '\n';
}

// =====================================================================
// FILE 2 - foundation (non-color)
// =====================================================================
function buildFoundation() {
  const out = [];
  out.push('# Fluent UI v8 (Fluent 1) - foundation', '');
  out.push('Не-цветовые значения темы Fluent UI v8 из `@fluentui/theme` '
    + `(версия ${themeVersion}). Светлая и темная схемы совпадают (генератор это проверяет).`, '');

  const table = (title, obj, note) => {
    out.push(`## ${title}`, '');
    if (note) out.push(note, '');
    out.push(rows(['Токен', 'Значение'], Object.entries(obj).map(([k, v]) => [code(k), String(v)])), '');
  };

  out.push('## Типографика - ступени `theme.fonts`', '');
  out.push('Высоты строки у v8 нет: ступень задает только семейство, размер и вес.', '');
  out.push(rows(['Ступень', 'Font family', 'Weight', 'Size'],
    Object.entries(L.fonts).map(([k, s]) => [code(k), family(s.fontFamily), s.fontWeight, s.fontSize])), '');
  table('Типографика - размеры шрифта (FontSizes)', T.FontSizes);
  table('Типографика - начертание (FontWeights)', T.FontWeights);
  table('Типографика - размеры иконок (IconFontSizes)', T.IconFontSizes);
  table('Типографика - локализованные семейства (LocalizedFontFamilies)',
    Object.fromEntries(Object.entries(T.LocalizedFontFamilies).map(([k, v]) => [k, family(v)])));

  const fx = L.effects;
  table('Скругления (effects)',
    Object.fromEntries(Object.keys(fx).filter(k => /^roundedCorner/.test(k)).map(k => [k, fx[k]])));
  table('Тени (effects)',
    Object.fromEntries(Object.keys(fx).filter(k => /^elevation/.test(k)).map(k => [k, fx[k]])));
  table('Тени (Depths)', T.Depths);
  table('Отступы (spacing)', L.spacing);
  table('Движение - длительности (MotionDurations)', T.MotionDurations);
  table('Движение - кривые (MotionTimings)', T.MotionTimings);
  table('Движение - переменные анимаций (AnimationVariables)', T.AnimationVariables);

  out.push('## Движение - готовые анимации (MotionAnimations)', '');
  out.push('Имена готовых keyframe-анимаций. Скалярных значений у них нет, длительности и '
    + 'кривые берутся из таблиц выше.', '');
  out.push(Object.keys(T.MotionAnimations).map(code).join(', '), '');

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
console.log(`done (@fluentui/theme ${themeVersion}, ${DARK_SOURCE.pkg} ${darkVersion}).`);
