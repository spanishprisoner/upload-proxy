import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { JQUERY_PROVIDER } from './shared/jquery.service';
import { AppComponent } from './app.component';
import { NavBarComponent } from './nav-bar.component';
import { SharedModule } from './shared/shared.module';
import { AppRoutingModule } from './app-routing.module';
import { AuthModule } from './auth/auth.module';
import { StoreModule } from '@ngrx/store';
import { appReducers } from './store/reducers/app.reducers';
import { FileUploadComponent } from './file-upload/file-upload.component';
import { FileUploadModule } from 'ng2-file-upload';
import { UploadedFilenamePipe } from './file-upload/uploaded-filename.pipe';
import { UploadedLinkPipe } from './file-upload/uploaded-link.pipe';

@NgModule({
  declarations: [
    AppComponent,
    NavBarComponent,
    FileUploadComponent,
    UploadedFilenamePipe,
    UploadedLinkPipe
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    SharedModule,
    AuthModule,
    FileUploadModule,
    StoreModule.forRoot(appReducers),
    AppRoutingModule
  ],
  providers: [JQUERY_PROVIDER],
  bootstrap: [AppComponent]
})
export class AppModule { }
