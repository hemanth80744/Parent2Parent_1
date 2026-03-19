import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { API_BASE_URL } from './api.config';
import { ApiResponse } from './api.types';
import { ParentProfile } from './models';

@Injectable({ providedIn: 'root' })
export class ParentsService {
  constructor(
    private readonly http: HttpClient
  ) {}

  /**
   * Search parents by school name.
   * - Uses backend API when available
   * - Falls back to demo data for offline UI demo
   */
  async searchBySchoolName(schoolName: string): Promise<ParentProfile[]> {
    const q = schoolName.trim().toLowerCase();
    if (!q) return [];

    type ApiItem = { userId: number; name: string; childClass: string | null };
    const res = await firstValueFrom(
      this.http.get<ApiResponse<ApiItem[]>>(`${API_BASE_URL}/api/users/search`, {
        params: { schoolName }
      })
    );

    if (!res?.success) throw new Error(res?.message ?? 'Search failed');

    // SP returns Id, Name, ChildClass (no CurrentSchoolName), so show searched name.
    return (res.data ?? []).map((x) => ({
      id: x.userId,
      name: x.name,
      childClass: x.childClass ?? '',
      currentSchoolName: schoolName
    }));
  }
}

