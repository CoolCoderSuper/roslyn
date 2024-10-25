﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Navigation;

internal interface IDocumentNavigationService : IWorkspaceService
{
    /// <summary>
    /// Determines whether it is possible to navigate to the given position in the specified document.
    /// </summary>
    /// <remarks>Legal to call from any thread.</remarks>
    Task<bool> CanNavigateToSpanAsync(Workspace workspace, DocumentId documentId, TextSpan textSpan, bool allowInvalidSpan, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether it is possible to navigate to the given virtual position in the specified document.
    /// </summary>
    /// <remarks>Legal to call from any thread.</remarks>
    Task<bool> CanNavigateToPositionAsync(Workspace workspace, DocumentId documentId, int position, int virtualSpace, bool allowInvalidPosition, CancellationToken cancellationToken);

    Task<INavigableLocation?> GetLocationForSpanAsync(Workspace workspace, DocumentId documentId, TextSpan textSpan, bool allowInvalidSpan, CancellationToken cancellationToken);
    Task<INavigableLocation?> GetLocationForPositionAsync(Workspace workspace, DocumentId documentId, int position, int virtualSpace, bool allowInvalidPosition, CancellationToken cancellationToken);
}

internal abstract class AbstractDocumentNavigationService : IDocumentNavigationService
{
    public virtual Task<bool> CanNavigateToSpanAsync(Workspace workspace, DocumentId documentId, TextSpan textSpan, bool allowInvalidSpan, CancellationToken cancellationToken)
        => SpecializedTasks.False;

    public virtual Task<bool> CanNavigateToPositionAsync(Workspace workspace, DocumentId documentId, int position, int virtualSpace, bool allowInvalidPosition, CancellationToken cancellationToken)
        => CanNavigateToSpanAsync(workspace, documentId, new TextSpan(position, 0), allowInvalidSpan: allowInvalidPosition, cancellationToken);

    public virtual Task<INavigableLocation?> GetLocationForSpanAsync(Workspace workspace, DocumentId documentId, TextSpan textSpan, bool allowInvalidSpan, CancellationToken cancellationToken)
        => SpecializedTasks.Null<INavigableLocation>();

    public virtual Task<INavigableLocation?> GetLocationForPositionAsync(Workspace workspace, DocumentId documentId, int position, int virtualSpace, bool allowInvalidPosition, CancellationToken cancellationToken)
        => GetLocationForSpanAsync(workspace, documentId, new TextSpan(position, 0), allowInvalidSpan: allowInvalidPosition, cancellationToken);
}

internal static class IDocumentNavigationServiceExtensions
{
    public static Task<bool> CanNavigateToSpanAsync(this IDocumentNavigationService service, Workspace workspace, DocumentId documentId, TextSpan textSpan, CancellationToken cancellationToken)
        => service.CanNavigateToSpanAsync(workspace, documentId, textSpan, allowInvalidSpan: false, cancellationToken);

    public static Task<bool> CanNavigateToPositionAsync(this IDocumentNavigationService service, Workspace workspace, DocumentId documentId, int position, CancellationToken cancellationToken)
        => service.CanNavigateToPositionAsync(workspace, documentId, position, virtualSpace: 0, allowInvalidPosition: false, cancellationToken);

    public static Task<INavigableLocation?> GetLocationForSpanAsync(this IDocumentNavigationService service, Workspace workspace, DocumentId documentId, TextSpan textSpan, CancellationToken cancellationToken)
        => service.GetLocationForSpanAsync(workspace, documentId, textSpan, allowInvalidSpan: false, cancellationToken);

    public static Task<INavigableLocation?> GetLocationForPositionAsync(this IDocumentNavigationService service, Workspace workspace, DocumentId documentId, int position, CancellationToken cancellationToken)
        => service.GetLocationForPositionAsync(workspace, documentId, position, virtualSpace: 0, allowInvalidPosition: false, cancellationToken);
}
