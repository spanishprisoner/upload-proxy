import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'uploadedLink'
})
export class UploadedLinkPipe implements PipeTransform {
  transform(value: string, args?: any): string {
    return value.split('`')[1];
  }
}
