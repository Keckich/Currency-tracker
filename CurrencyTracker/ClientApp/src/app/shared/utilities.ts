export function format(template: string, values: Record<string, string | number>): string {
  return template.replace(/{(\w+)}/g, (_, key) => values[key]?.toString() ?? '');
}
