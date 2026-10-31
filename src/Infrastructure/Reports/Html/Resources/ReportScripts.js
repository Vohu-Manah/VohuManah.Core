let currentZoom = 100;
function getAccessToken() {
    try {
        const urlParams = new URLSearchParams(window.location.search);
        const tokenFromUrl = urlParams.get('access_token');
        if (tokenFromUrl) {
            return tokenFromUrl;
        }
        
        if (window.parent && window.parent !== window) {
            try {
                const parentToken = window.parent.localStorage.getItem('token');
                if (parentToken) {
                    const tokenData = JSON.parse(parentToken);
                    return tokenData.accessToken || null;
                }
            } catch (e) {
                try {
                    window.parent.postMessage({ type: 'getToken' }, '*');
                } catch (e2) {
                }
            }
        }
        
        const token = localStorage.getItem('vm_auth_tokens');
        if (token) {
            const tokenData = JSON.parse(token);
            return tokenData.accessToken || null;
        }
    } catch (e) {
        console.error('خطا در گرفتن اکسس توکن:', e);
    }
    return null;
}

function exportPDF(reportName, params) {
    if (window.parent && window.parent !== window) {
        window.parent.postMessage({ type: 'report:export', format: 'pdf' }, '*');
    }
}

function exportExcel(reportName, params) {
    if (window.parent && window.parent !== window) {
        window.parent.postMessage({ type: 'report:export', format: 'excel' }, '*');
    }
}

function zoomIn() {
    currentZoom = Math.min(currentZoom + 10, 200);
    applyZoom();
}

function zoomOut() {
    currentZoom = Math.max(currentZoom - 10, 50);
    applyZoom();
}

function resetZoom() {
    currentZoom = 100;
    applyZoom();
}

function applyZoom() {
    const reportContent = document.getElementById('reportContent');
    if (reportContent) {
        reportContent.style.transform = `scale(${currentZoom / 100})`;
        reportContent.style.transformOrigin = 'top center';
        const zoomValue = document.getElementById('zoomValue');
        if (zoomValue) {
            zoomValue.textContent = currentZoom + '%';
        }
    } else {
        console.warn('reportContent element not found for zoom');
    }
}


function printReport() {
    
    if (window.parent && window.parent !== window) {
        window.parent.postMessage({ type: 'report:print' }, '*');
        return;
    }
    window.print();
}




let currentPage = 1;
let totalPages = 1;
let itemsPerPage = 50;
let allRowsCache = []; 



const serverPaging = typeof params !== 'undefined' && params && params.TotalPages && params.Page;
let serverPage = serverPaging ? Number(params.Page) : 1;
let serverTotalPages = serverPaging ? Number(params.TotalPages) : 1;
let serverTotalCount = serverPaging ? Number(params.TotalCount) : 0;

function requestServerPage(pageNumber) {
    
    if (window.parent && window.parent !== window) {
        window.parent.postMessage({ type: 'report:page', page: pageNumber }, '*');
    }
}

function initPagination() {
    const reportContent = document.getElementById('reportContent');
    if (!reportContent) {
        console.warn('Report content not found');
        return;
    }
    
    const tables = reportContent.querySelectorAll('table');
    if (tables.length === 0) {
        console.warn('No tables found in report');
        allRowsCache = [];
        totalPages = 1;
        currentPage = 1;
        updatePaginationUI();
        return;
    }
    
    
    
    allRowsCache = [];
    tables.forEach(table => {
        const tbodyRows = table.querySelectorAll('tbody tr');
        if (tbodyRows.length > 0) {
            allRowsCache.push(...Array.from(tbodyRows));
        } else {
            
            const allRows = table.querySelectorAll('tr');
            const thead = table.querySelector('thead');
            allRows.forEach(row => {
                if (!thead || !thead.contains(row)) {
                    allRowsCache.push(row);
                }
            });
        }
    });
    
    if (allRowsCache.length === 0) {
        console.warn('No data rows found in tables');
        totalPages = 1;
        currentPage = 1;
        updatePaginationUI();
        return;
    }
    
    
    const visibleRows = allRowsCache.filter(row => 
        !row.classList.contains('search-hidden')
    );
    
    totalPages = Math.max(1, Math.ceil(visibleRows.length / itemsPerPage));
    currentPage = serverPaging ? serverPage : 1;

    console.log(`Pagination initialized: ${visibleRows.length} rows, ${serverPaging ? serverTotalPages : totalPages} pages`);

    updatePaginationUI();
    if (!serverPaging) {
        showPage(currentPage);
    }
}

function showPage(page) {
    const reportContent = document.getElementById('reportContent');
    if (!reportContent) return;
    
    
    if (allRowsCache.length === 0) {
        const tables = reportContent.querySelectorAll('table');
        if (tables.length === 0) return;
        
        allRowsCache = Array.from(tables).flatMap(table => 
            Array.from(table.querySelectorAll('tbody tr'))
        );
    }
    
    
    const visibleRows = allRowsCache.filter(row => 
        !row.classList.contains('search-hidden')
    );
    
    if (visibleRows.length === 0) {
        
        allRowsCache.forEach(row => {
            if (!row.classList.contains('search-hidden')) {
                row.style.display = '';
            }
        });
        currentPage = 1;
        totalPages = 1;
        updatePaginationUI();
        return;
    }
    
    
    totalPages = Math.max(1, Math.ceil(visibleRows.length / itemsPerPage));
    
    
    if (page < 1) page = 1;
    if (page > totalPages) page = totalPages;
    
    const startIndex = (page - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    
    
    allRowsCache.forEach(row => {
        if (row.classList.contains('search-hidden')) {
            
            row.style.display = 'none';
        } else {
            
            const rowIndex = visibleRows.indexOf(row);
            if (rowIndex >= startIndex && rowIndex < endIndex) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        }
    });
    
    currentPage = page;
    updatePaginationUI();
}

function updatePaginationUI() {
    const paginationDiv = document.getElementById('paginationControls');
    if (!paginationDiv) {
        console.warn('Pagination controls not found in HTML');
        return;
    }
    
    
    const activePage = serverPaging ? serverPage : currentPage;
    const lastPage = serverPaging ? serverTotalPages : totalPages;

    
    const pageInfo = paginationDiv.querySelector('.pagination-info');
    if (pageInfo) {
        const total = serverPaging && serverTotalCount
            ? ` (${serverTotalCount.toLocaleString('fa-IR')} رکورد)`
            : '';
        pageInfo.textContent = `صفحه ${activePage.toLocaleString('fa-IR')} از ${lastPage.toLocaleString('fa-IR')}${total}`;
    }
    
    
    const buttons = paginationDiv.querySelectorAll('.pagination-btn');
    if (buttons.length >= 4) {
        
        buttons[0].disabled = activePage === 1;
        
        
        buttons[1].disabled = activePage === 1;
        
        
        buttons[2].disabled = activePage === lastPage;
        
        
        buttons[3].disabled = activePage === lastPage;
    }
}

function goToFirstPage() {
    if (serverPaging) {
        if (serverPage > 1) requestServerPage(1);
        return;
    }
    if (currentPage > 1) showPage(1);
}

function goToPreviousPage() {
    if (serverPaging) {
        if (serverPage > 1) requestServerPage(serverPage - 1);
        return;
    }
    if (currentPage > 1) showPage(currentPage - 1);
}

function goToNextPage() {
    if (serverPaging) {
        if (serverPage < serverTotalPages) requestServerPage(serverPage + 1);
        return;
    }
    if (currentPage < totalPages) showPage(currentPage + 1);
}

function goToLastPage() {
    if (serverPaging) {
        if (serverPage < serverTotalPages) requestServerPage(serverTotalPages);
        return;
    }
    if (currentPage < totalPages) showPage(totalPages);
}


let searchTerm = '';
let searchResults = [];
let currentSearchIndex = -1;

function initSearch() {
    
    const searchDiv = document.getElementById('searchControls');
    if (!searchDiv) {
        console.warn('Search controls not found in HTML');
        return;
    }
    
    
    const searchInput = document.getElementById('searchInput');
    if (searchInput && !searchInput.hasAttribute('data-initialized')) {
        searchInput.setAttribute('data-initialized', 'true');
        searchInput.addEventListener('keyup', handleSearchKeyup);
    }
}

function handleSearchKeyup(event) {
    if (event.key === 'Enter') {
        performSearch();
    } else if (event.key === 'Escape') {
        clearSearch();
    }
}

function performSearch() {
    const searchInput = document.getElementById('searchInput');
    if (!searchInput) return;
    
    searchTerm = searchInput.value.trim();
    
    if (!searchTerm) {
        clearSearch();
        return;
    }
    
    const reportContent = document.getElementById('reportContent');
    if (!reportContent) return;
    
    
    clearHighlights();
    
    
    if (allRowsCache.length === 0) {
        const tables = reportContent.querySelectorAll('table');
        allRowsCache = Array.from(tables).flatMap(table => {
            const tbodyRows = table.querySelectorAll('tbody tr');
            if (tbodyRows.length > 0) {
                return Array.from(tbodyRows);
            } else {
                const allRows = table.querySelectorAll('tr');
                const thead = table.querySelector('thead');
                return Array.from(allRows).filter(row => !thead || !thead.contains(row));
            }
        });
    }
    
    
    
    allRowsCache.forEach(row => {
        row.classList.remove('search-hidden');
    });
    
    
    searchResults = [];
    const walker = document.createTreeWalker(
        reportContent,
        NodeFilter.SHOW_TEXT,
        null,
        false
    );
    
    let node;
    while (node = walker.nextNode()) {
        const text = node.textContent;
        const index = text.toLowerCase().indexOf(searchTerm.toLowerCase());
        
        if (index !== -1) {
            searchResults.push({
                node: node,
                text: text,
                index: index
            });
        }
    }
    
    
    allRowsCache.forEach(row => {
        const rowText = row.textContent.toLowerCase();
        if (!rowText.includes(searchTerm.toLowerCase())) {
            row.classList.add('search-hidden');
        } else {
            row.classList.remove('search-hidden');
        }
    });
    
    
    highlightMatches();
    
    
    const visibleRows = allRowsCache.filter(row => 
        !row.classList.contains('search-hidden')
    );
    
    totalPages = Math.max(1, Math.ceil(visibleRows.length / itemsPerPage));
    currentPage = 1;
    
    
    
    showPage(currentPage);
    
    
    updatePaginationUI();
    updateSearchUI();
    
    
    if (searchResults.length > 0) {
        currentSearchIndex = 0;
        scrollToResult(0);
    }
}

function highlightMatches() {
    if (searchResults.length === 0 || !searchTerm) return;
    
    const reportContent = document.getElementById('reportContent');
    if (!reportContent) return;
    
    
    const walker = document.createTreeWalker(
        reportContent,
        NodeFilter.SHOW_TEXT,
        null,
        false
    );
    
    const textNodes = [];
    let node;
    while (node = walker.nextNode()) {
        if (node.textContent.toLowerCase().includes(searchTerm.toLowerCase())) {
            textNodes.push(node);
        }
    }
    
    textNodes.forEach(textNode => {
        const parent = textNode.parentNode;
        if (!parent || parent.tagName === 'MARK' || parent.tagName === 'SCRIPT' || parent.tagName === 'STYLE') {
            return;
        }
        
        const text = textNode.textContent;
        const regex = new RegExp(`(${searchTerm.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')})`, 'gi');

        
        
        
        
        
        const matches = Array.from(text.matchAll(regex));
        if (matches.length === 0) {
            return;
        }

        const fragment = document.createDocumentFragment();
        let cursor = 0;

        matches.forEach(match => {
            const start = match.index;
            const end = start + match[0].length;

            if (start > cursor) {
                fragment.appendChild(document.createTextNode(text.slice(cursor, start)));
            }

            const mark = document.createElement('mark');
            mark.className = 'search-highlight';
            mark.textContent = match[0];
            fragment.appendChild(mark);

            cursor = end;
        });

        if (cursor < text.length) {
            fragment.appendChild(document.createTextNode(text.slice(cursor)));
        }

        parent.replaceChild(fragment, textNode);
    });
    
    
    searchResults = [];
    const highlights = reportContent.querySelectorAll('.search-highlight');
    highlights.forEach((highlight, index) => {
        searchResults.push({
            element: highlight,
            index: index
        });
    });
}

function clearHighlights() {
    const reportContent = document.getElementById('reportContent');
    if (!reportContent) return;
    
    const highlights = reportContent.querySelectorAll('.search-highlight');
    highlights.forEach(highlight => {
        const parent = highlight.parentNode;
        if (parent) {
            const textNode = document.createTextNode(highlight.textContent);
            parent.replaceChild(textNode, highlight);
        }
    });
    
    
    const walker = document.createTreeWalker(
        reportContent,
        NodeFilter.SHOW_TEXT,
        null,
        false
    );
    
    let node;
    const nodesToNormalize = [];
    while (node = walker.nextNode()) {
        if (node.parentNode) {
            nodesToNormalize.push(node.parentNode);
        }
    }
    
    nodesToNormalize.forEach(parent => {
        parent.normalize();
    });
}

function clearSearch() {
    searchTerm = '';
    searchResults = [];
    currentSearchIndex = -1;
    
    const searchInput = document.getElementById('searchInput');
    if (searchInput) searchInput.value = '';
    
    
    allRowsCache.forEach(row => {
        row.classList.remove('search-hidden');
    });
    
    clearHighlights();
    updateSearchUI();
    
    
    initPagination();
}

function updateSearchUI() {
    const clearBtn = document.getElementById('clearSearchBtn');
    const prevBtn = document.getElementById('prevResultBtn');
    const nextBtn = document.getElementById('nextResultBtn');
    const resultsInfo = document.getElementById('searchResultsInfo');
    
    if (!clearBtn || !prevBtn || !nextBtn || !resultsInfo) {
        return;
    }
    
    if (searchResults.length > 0) {
        clearBtn.style.display = 'inline-block';
        prevBtn.style.display = 'inline-block';
        nextBtn.style.display = 'inline-block';
        resultsInfo.textContent = `${currentSearchIndex + 1} از ${searchResults.length}`;
        resultsInfo.style.display = 'inline-block';
    } else if (searchTerm) {
        clearBtn.style.display = 'inline-block';
        prevBtn.style.display = 'none';
        nextBtn.style.display = 'none';
        resultsInfo.textContent = 'نتیجه‌ای یافت نشد';
        resultsInfo.style.display = 'inline-block';
    } else {
        clearBtn.style.display = 'none';
        prevBtn.style.display = 'none';
        nextBtn.style.display = 'none';
        resultsInfo.style.display = 'none';
    }
}

function goToPreviousResult() {
    if (searchResults.length === 0) return;
    currentSearchIndex = (currentSearchIndex - 1 + searchResults.length) % searchResults.length;
    scrollToResult(currentSearchIndex);
    updateSearchUI();
}

function goToNextResult() {
    if (searchResults.length === 0) return;
    currentSearchIndex = (currentSearchIndex + 1) % searchResults.length;
    scrollToResult(currentSearchIndex);
    updateSearchUI();
}

function scrollToResult(index) {
    if (index < 0 || index >= searchResults.length) return;
    
    const reportContent = document.getElementById('reportContent');
    if (!reportContent) return;
    
    const highlights = reportContent.querySelectorAll('.search-highlight');
    if (highlights[index]) {
        highlights[index].scrollIntoView({ behavior: 'smooth', block: 'center' });
        highlights[index].classList.add('search-highlight-active');
        
        
        highlights.forEach((h, i) => {
            if (i !== index) h.classList.remove('search-highlight-active');
        });
    }
}

document.addEventListener('keydown', function(e) {
    if (e.ctrlKey || e.metaKey) {
        if (e.key === '=' || e.key === '+') {
            e.preventDefault();
            zoomIn();
        } else if (e.key === '-') {
            e.preventDefault();
            zoomOut();
        } else if (e.key === '0') {
            e.preventDefault();
            resetZoom();
        } else if (e.key === 'p') {
            e.preventDefault();
            printReport();
        } else if (e.key === 'f') {
            e.preventDefault();
            const searchInput = document.getElementById('searchInput');
            if (searchInput) searchInput.focus();
        }
    } else if (e.key === 'F3') {
        e.preventDefault();
        if (searchResults.length > 0) {
            goToNextResult();
        } else {
            performSearch();
        }
    }
});

function initializeReportFeatures() {
    applyZoom();
    initPagination();
    initSearch();
}

document.addEventListener('DOMContentLoaded', function() {
    
    setTimeout(initializeReportFeatures, 100);
    
    
    setTimeout(function() {
        const reportContent = document.getElementById('reportContent');
        if (reportContent && allRowsCache.length === 0) {
            console.log('Retrying pagination initialization...');
            initPagination();
        }
    }, 500);
    
    
    const reportContent = document.getElementById('reportContent');
    if (reportContent) {
        const observer = new MutationObserver(function(mutations) {
            let shouldReinit = false;
            mutations.forEach(function(mutation) {
                if (mutation.addedNodes.length > 0) {
                    
                    mutation.addedNodes.forEach(function(node) {
                        if (node.nodeType === 1) { 
                            if (node.tagName === 'TABLE' || node.querySelector('table')) {
                                shouldReinit = true;
                            }
                        }
                    });
                }
            });
            
            if (shouldReinit && allRowsCache.length === 0) {
                console.log('Content changed, reinitializing pagination...');
                setTimeout(initPagination, 100);
            }
        });
        
        observer.observe(reportContent, {
            childList: true,
            subtree: true
        });
    }
});

