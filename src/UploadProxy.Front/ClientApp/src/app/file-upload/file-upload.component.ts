import { Component, OnInit } from '@angular/core';
import { FileUploader, FileItem, ParsedResponseHeaders } from 'ng2-file-upload';

@Component({
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css']
})
export class FileUploadComponent implements OnInit {
  public uploader: FileUploader = new FileUploader({ url: '/' });
  public hasBaseDropZoneOver = false;
  public hasAnotherDropZoneOver = false;

  ngOnInit(): void {
    this.uploader.onCompleteItem = (item: FileItem, response: string,
      status: Number, headers: ParsedResponseHeaders) => {
        item.file.name = '<a href="url">link text</a>';
        console.log('debug');
      };
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }
}
