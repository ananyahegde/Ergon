import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const authService = inject(AuthService);

  const cloneWithToken = (token: string) => req.clone({
    setHeaders: { Authorization: `Bearer ${token}` }
  });

  const token = authService.getToken();
  const authReq = token ? cloneWithToken(token) : req;

  return next(authReq).pipe(
    catchError((error) => {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        const refreshToken = localStorage.getItem('refresh_token');
        if (refreshToken) {
          return authService.refresh(refreshToken).pipe(
            switchMap((response) => {
              return next(cloneWithToken(response.accessToken));
            }),
            catchError((refreshError) => {
              authService.logout();
              return throwError(() => refreshError);
            })
          );
        }
        authService.logout();
      }
      return throwError(() => error);
    })
  );
};
