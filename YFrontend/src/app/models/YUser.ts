import { YProfile } from "./YProfile";

export interface YUser {
    guid: string;
    username: string;
    email: string;
    createdAt: Date;
    lastLogin: Date | null;
    profile: YProfile;
}