import { Pipe, PipeTransform } from '@angular/core';
import { orderBy } from 'lodash';

@Pipe({
  name: 'sort',
  standalone: true
})
export class SortPipe implements PipeTransform {

  transform<T>(array: T[], field: string, order: 'asc' | 'desc' = 'asc'): T[] {
    if (!array || !field) {
      return array;
    }

    return orderBy(array, [field], [order]);
  }
}
