import api from '@/services/api'

export interface TrainingMoveOption {
  openingNodeId: number
  moveSan: string
  moveUci?: string | null
}

export interface GetNextTrainingPositionResponse {
  openingNodeId: number
  openingId: number
  openingName: string
  fen: string
  sideToMove: string
  repertoireColor: number
  moveOptions: TrainingMoveOption[]
}

export interface GetTrainingSummaryResponse {
  totalPositions: number
  newPositions: number
  duePositions: number
}

export interface SubmitTrainingAnswerRequest {
  openingNodeId: number
  selectedOpeningNodeId: number
}

export interface SubmitTrainingAnswerResponse {
  isCorrect: boolean
  correctOpeningNodeId: number
  correctMoveSan: string
  correctMoveUci?: string | null
  bucket: number
  trainedCount: number
  failedCount: number
  nextDueAtUtc?: string | null
  currentOpeningNodeId: number
  currentFen: string
  currentSideToMove: string
  moveOptions: TrainingMoveOption[]
}

export async function getTrainingSummary(): Promise<GetTrainingSummaryResponse> {
  const { data } = await api.get<GetTrainingSummaryResponse>('/training/summary')
  return data
}

export async function getNextTrainingPosition(): Promise<GetNextTrainingPositionResponse | null> {
  const { data } = await api.get<GetNextTrainingPositionResponse | null>('/training/next-position')
  return data
}

export async function submitTrainingAnswer(
  payload: SubmitTrainingAnswerRequest
): Promise<SubmitTrainingAnswerResponse> {
  const { data } = await api.post<SubmitTrainingAnswerResponse>('/training/submit-answer', payload)
  return data
}