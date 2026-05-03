import api from '@/services/api'

export enum LineType {
  Main = 1,
  Side = 2,
  Other = 3
}

export interface RepertoireItem {
  id: string
  parentId?: string | null
  name: string
  color: number
  openingId?: number | null
}

export type CandidateMove = {
  nodeId: number
  moveSan: string
  moveUci: string
  openingId: number
  openingName: string
  isFromCurrentOpening: boolean
  lineType: LineType
}

export interface UpdateCandidateMoveLineTypeRequest {
  openingId: number
  nodeId: number
  lineType: LineType
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

export async function deleteOpeningNodeSubtree(
  openingId: number, 
  nodeId: number
) {
  await api.delete(`/openings/${openingId}/nodes/${nodeId}`)
}

export async function updateCandidateMoveLineType(request: UpdateCandidateMoveLineTypeRequest) {
  const { data } = await api.patch(
    `/openings/${request.openingId}/nodes/${request.nodeId}/type`,
    {
      lineType: request.lineType
    }
  )

  return data
}
