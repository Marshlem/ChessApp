import api from '@/services/api'

export interface RepertoireItem {
  id: string
  parentId?: string | null
  name: string
  color: number
  openingId?: number | null
}

export type CandidateMove = {
  moveSan: string
  moveUci: string
  openingId: number
  openingName: string
  isFromCurrentOpening: boolean
}

export async function getRepertoireTree(): Promise<RepertoireItem[]> {
  const { data } = await api.get('/repertoire')
  return data
}

export async function createOpening(payload: {
  parentId?: string | null
  name: string
  color: number
}) {
  const { data } = await api.post('/repertoire/opening', payload)
  return data as string // openingId
}

export async function getCandidateMoves(params: {
  fen: string
  currentOpeningId?: number
}) {
  const { data } = await api.get<CandidateMove[]>('/candidate-moves', {
    params: {
      fen: params.fen,
      currentOpeningId: params.currentOpeningId
    }
  })

  return data
}
