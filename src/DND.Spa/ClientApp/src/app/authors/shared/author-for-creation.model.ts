import { AuthorAbstractBase } from "./author-abstract-base.model";
import { ShowForCreation } from "../shows/shared/show-for-creation.model";

export class AuthorForCreation extends AuthorAbstractBase {
  shows: ShowForCreation[];
}
