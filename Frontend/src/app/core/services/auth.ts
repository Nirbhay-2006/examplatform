import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = '/api/auth';
  private currentUserSubject: BehaviorSubject<any>;
  public currentUser: Observable<any>;
  private isPremiumSubject = new BehaviorSubject<boolean>(false);
  public isPremium$ = this.isPremiumSubject.asObservable();

  constructor(private http: HttpClient) {
    const storedUser = localStorage.getItem('currentUser');
    if (storedUser) {
      const user = JSON.parse(storedUser);
      this.currentUserSubject = new BehaviorSubject<any>(user);
      this.updateState(user);
    } else {
      this.currentUserSubject = new BehaviorSubject<any>(null);
    }
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): any {
    return this.currentUserSubject.value;
  }

  login(credentials: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials)
      .pipe(tap(user => {
        // store user details and jwt token in local storage
        localStorage.setItem('currentUser', JSON.stringify(user));
        this.currentUserSubject.next(user);
        this.updateState(user);
      }));
  }

  private updateState(user: any) {
    if (user && user.token) {
      try {
        const decoded: any = jwtDecode(user.token);
        const subscriptionType = decoded.subscriptionType || 'free'; // usage depends on Token Claim name
        this.isPremiumSubject.next(subscriptionType === 'paid');
      } catch (e) {
        console.error('Failed to decode token', e);
        this.isPremiumSubject.next(false);
      }
    } else {
      this.isPremiumSubject.next(false);
    }
  }

  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, user);
  }

  logout() {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
    this.isPremiumSubject.next(false);
  }
}
