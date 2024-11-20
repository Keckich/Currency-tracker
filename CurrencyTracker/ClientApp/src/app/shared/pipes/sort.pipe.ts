import { Pipe, PipeTransform } from '@angular/core';
import { orderBy } from 'lodash';
import { SortOrder } from '../shared.model';

@Pipe({
  name: 'sort',
  standalone: true
})
export class SortPipe implements PipeTransform {

  transform<T>(array: T[], field: string, order: SortOrder = SortOrder.Asc): T[] {
    if (!array || !field) {
      return array;
    }

    return orderBy(array, [field], [order]);
  }
}
