export class User {
  constructor(public userId: bigint = 0n, public username: string = '', public email: string = '', public phone: string = '', public role: string = '', public token: string = '', public lastActive: Date = new Date()) { }
}
