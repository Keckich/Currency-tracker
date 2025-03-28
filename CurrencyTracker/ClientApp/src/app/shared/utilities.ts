export function format(template: string, values: Record<string, string | number>): string {
  return template.replace(/{(\w+)}/g, (_, key) => values[key]?.toString() ?? '');
}

export function formatString(template: string, ...args: any[]): string {
  return template.replace(/{(\d+)}/g, (match: string, index: number): string => {
    return args[index] !== undefined ? String(args[index]) : '';
  });
}
