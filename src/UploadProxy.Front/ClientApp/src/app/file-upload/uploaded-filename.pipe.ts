import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'uploadedFilename'
})
export class UploadedFilenamePipe implements PipeTransform {
  transform(value: string, args?: any): string {
    return value.split('`')[0];
  }
}
