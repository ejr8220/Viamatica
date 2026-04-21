export interface Cash {
  cashId: number;
  name: string;
  code: string;
  description: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateCashRequest {
  name: string;
  code: string;
  description: string;
}

export interface UpdateCashRequest {
  name: string;
  description: string;
  isActive: boolean;
}
