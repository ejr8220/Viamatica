export interface Attention {
  attentionId: number;
  turnId: number;
  turnNumber: string;
  cashId: number;
  cashName: string;
  userId: number;
  userName: string;
  clientName: string;
  serviceName: string;
  startTime: string;
  endTime?: string;
  status: string;
  notes?: string;
}

export interface CreateAttentionRequest {
  turnId: number;
  cashId: number;
  notes?: string;
}

export interface UpdateAttentionRequest {
  status: string;
  notes?: string;
}
