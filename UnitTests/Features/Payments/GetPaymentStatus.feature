Feature: Obter Status de Pagamento

  Como um usuário do sistema,
  Eu quero consultar o status de pagamento de um pedido,
  Para que eu possa saber se o pagamento foi aprovado, pendente ou outro status.

  Background:
    Dado que existe um repositório de pagamentos disponível

  Scenario: Deveria retornar o status de pagamento quando o pagamento existe
    Dado que existe um pagamento aprovado com o OrderId 123
    Quando eu consultar o status de pagamento para o pedido com OrderId 123
    Então o status do pagamento deve ser "Aprovado"
    E o número do pedido deve ser "12345"
    E a data de processamento do pagamento deve ser uma data válida

  Scenario: Deveria retornar nulo quando o pagamento não existir
    Dado que não existe nenhum pagamento com o OrderId 999
    Quando eu consultar o status de pagamento para o pedido com OrderId 999
    Então o status de pagamento deve ser nulo
