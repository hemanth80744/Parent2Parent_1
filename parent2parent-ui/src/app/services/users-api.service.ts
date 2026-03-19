import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { API_BASE_URL } from './api.config';
import { ApiResponse } from './api.types';

export type RegisterDto = {
  name: string;
  username: string;
  password: string;
  school: string;
  class: string;
};

export type LoginDto = {
  username: string;
  password: string;
};

export type AuthResponseDto = {
  userId: number;
  name: string;
};

@Injectable({ providedIn: 'root' })
export class UsersApiService {
  constructor(private readonly http: HttpClient) {}

  async register(dto: RegisterDto): Promise<ApiResponse<unknown>> {
    return await firstValueFrom(
      this.http.post<ApiResponse<unknown>>(`${API_BASE_URL}/api/users/register`, dto)
    );
  }

  async login(dto: LoginDto): Promise<ApiResponse<AuthResponseDto>> {
    return await firstValueFrom(
      this.http.post<ApiResponse<AuthResponseDto>>(`${API_BASE_URL}/api/users/login`, dto)
    );
  }
}

