export interface CurrentUser {
  isAuthenticated: boolean;
  userName: string | null;
  displayName: string | null;
  roles: string[];
}
