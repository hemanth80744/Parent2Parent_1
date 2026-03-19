export type ParentProfile = {
  id: number;
  name: string;
  currentSchoolName: string;
  childClass: string;
  username?: string;
};

export type ConnectionRequest = {
  requestId: number;
  senderId: number;
  senderName: string;
  receiverId: number;
  receiverName?: string;
  status: string;
  createdAt?: string;
};

export type ChatMessage = {
  id: string;
  senderId: number;
  receiverId: number;
  message: string;
  sentAt: Date;
};

