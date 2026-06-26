import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';
import { catchError, switchMap, throwError, BehaviorSubject, filter, take } from 'rxjs';

let isRefreshing = false;
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

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

        if (!refreshToken) {
          authService.logout();
          return throwError(() => error);
        }

        if (isRefreshing) {
          return refreshTokenSubject.pipe(
            filter(token => token !== null),
            take(1),
            switchMap(token => next(cloneWithToken(token!)))
          );
        }

        isRefreshing = true;
        refreshTokenSubject.next(null);

        return authService.refresh(refreshToken).pipe(
          switchMap((response) => {
            isRefreshing = false;
            refreshTokenSubject.next(response.accessToken);
            return next(cloneWithToken(response.accessToken));
          }),
          catchError((refreshError) => {
            isRefreshing = false;
            refreshTokenSubject.next(null);
            authService.logout();
            return throwError(() => refreshError);
          })
        );
      }

      return throwError(() => error);
    })
  );
};