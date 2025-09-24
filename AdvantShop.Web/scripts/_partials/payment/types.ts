export type PaymentMethodType =
    | 'full_prepayment'
    | 'partial_prepayment'
    | 'advance'
    | 'full_payment'
    | 'partial_payment'
    | 'credit'
    | 'credit_payment';

export type PaymentSubjectType =
    | 'commodity'
    | 'excise'
    | 'job'
    | 'service'
    | 'gambling_bet'
    | 'gambling_prize'
    | 'lottery'
    | 'lottery_prize'
    | 'intellectual_activity'
    | 'payment'
    | 'agent_commission'
    | 'composite'
    | 'another';
