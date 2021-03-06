import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FileUploader, FileItem, ParsedResponseHeaders } from 'ng2-file-upload';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { ErrorType } from '../shared/error-type';
import { of } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Component({
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css']
})
export class FileUploadComponent implements OnInit {
  public uploader: FileUploader = new FileUploader({ url: '/api/upload', maxFileSize: 1024 * 1024 * 1024 });
  public hasBaseDropZoneOver = false;
  public hasAnotherDropZoneOver = false;
  public accessError = null;

  constructor(private httpClient: HttpClient, private authService: AuthService, private changeDetector: ChangeDetectorRef) {
  }

  ngOnInit(): void {
    this.uploader.setOptions({
      headers: [
        { name: 'Authorization', value: 'Bearer ' + this.authService.accessToken }
      ]
    });
    this.uploader.onCompleteItem = (item: FileItem, response: string,
      status: Number, headers: ParsedResponseHeaders) => {
      if (response && response.length > 0) {
        item.file.name += '`' + response;
      }
    };
    this.uploader.onProgressItem = (progress: any) => this.changeDetector.detectChanges();

    this.httpClient.get<void | ErrorType>('/api/auth/checkaccess')
      .pipe(
        catchError(e => of({ error: 'You do not have access to this resource' } as ErrorType))
      ).subscribe(e => {
        if (e && (e as ErrorType).error != null) {
          this.accessError = (e as ErrorType).error;
        } else {
          this.accessError = '';
        }
      });
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }
}
