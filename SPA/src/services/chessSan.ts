import { Chess } from 'chess.js'

const PROMOS = new Set(['q', 'r', 'b', 'n'])

export function uciToSan(fen: string, uci: string): string {
  const chess = new Chess(fen)
  const move = chess.move({
    from: uci.slice(0, 2),
    to: uci.slice(2, 4),
    promotion: uci[4]
  })
  return move?.san ?? '??'
}
