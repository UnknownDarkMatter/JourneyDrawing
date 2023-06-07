import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpHeaders,
  HttpResponse,
  HttpXsrfTokenExtractor,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, catchError, map, throwError } from 'rxjs';
import { LoadingService } from './loading.service';

@Injectable({
  providedIn: 'root',
})
export class AuthInterceptor implements HttpInterceptor {
  constructor(private loading: LoadingService) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    this.loading.setLoading(true, req.url);

    let authReq = req.clone({ withCredentials: true });

    if (req.url.indexOf('SPALogin') > 0) {
      return next.handle(authReq).pipe(
        map<HttpEvent<any>, any>((evt: HttpEvent<any>) => {
          if (evt instanceof HttpResponse) {
            this.loading.setLoading(false, req.url);
          }
          return evt;
        })
      );
    }

    return next.handle(authReq).pipe(
      catchError((err) => {
        this.loading.setLoading(false, req.url);
        return this.catchError(err);
      }),
      map<HttpEvent<any>, any>((evt: HttpEvent<any>) => {
        if (evt instanceof HttpResponse) {
          this.loading.setLoading(false, req.url);
        }
        return evt;
      })
    );
  }

  catchError(err: any): Observable<any> {
    console.error(err);
    return throwError(
      () => new Error('An error occured. Please contact an administrator')
    );
  }
}
