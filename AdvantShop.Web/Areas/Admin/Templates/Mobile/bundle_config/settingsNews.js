import appDependency from '../../../../../scripts/appDependency.js';
import '../../../Content/src/news/news.js';
import '../Content/styles/views/settings.scss';
appDependency.addItem(`news`);

import '../Content/styles/_shared/card/card.scss';
import '../../../Content/src/newsCategory/newsCategory.js';
import '../../../Content/src/newsCategory/modal/addEditNewsCategory/ModalAddEditNewsCategoryCtrl.js';
appDependency.addItem(`newsCategory`);
