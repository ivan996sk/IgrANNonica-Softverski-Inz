export default class User {
    _id?: string = '';
    constructor(
        public username: string = '',
        public email: string = '',
        public password: string = '',
        public firstName: string = '',
        public lastName: string = '',
        public photoId: string = '1', //difoltna profilna slika
        public isPermament:boolean=false,
        public dateCreated:Date=new Date()
    ) { }
}